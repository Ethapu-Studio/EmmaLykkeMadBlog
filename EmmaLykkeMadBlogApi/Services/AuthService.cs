using System.Security.Claims;
using System.Text;
using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Infrastructure;
using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EmmaLykkeMadBlog.Services;

// Identity's UserManager / SignInManager are used directly: they are the data and
// security layer for users (hashing, lockout, tokens, security stamps).
public class AuthService(
    UserManager<AppUser> users,
    SignInManager<AppUser> signIn,
    IAccountEmailService emails,
    IMemoryCache cache,
    IOptionsMonitor<BearerTokenOptions> bearer,
    IOptions<AuthOptions> options,
    TimeProvider time,
    ILogger<AuthService> logger) : IAuthService
{
    private static readonly TimeSpan MailCooldown = TimeSpan.FromMinutes(2);
    private static readonly AppUser DummyUser = new();
    private const string WeakPasswordMessage = "Adgangskoden skal være mindst 7 tegn.";

    // ---------- Register / confirm ----------

    public async Task<ServiceResult> RegisterAsync(string name, string email, string password, bool newsletter)
    {
        name = name.Trim();
        email = email.Trim();

        // Checked first and for everyone, so the answer never depends on whether the address exists.
        var passwordCheck = await ValidatePasswordAsync(password);
        if (!passwordCheck.Succeeded) return passwordCheck;

        var existing = await users.FindByEmailAsync(email);

        if (existing is { EmailConfirmed: true })
        {
            if (!OnCooldown("exists", email)) await emails.SendAlreadyRegisteredAsync(existing);
            return ServiceResult.Ok();
        }

        if (existing is not null)
        {
            // Unconfirmed: the latest submission wins. Old links die with the old account.
            if (OnCooldown("confirm", email)) return ServiceResult.Ok();
            await users.DeleteAsync(existing);
        }
        else
        {
            _ = OnCooldown("confirm", email); // starts the cool-down for this address
        }

        var now = time.GetUtcNow();
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            DisplayName = name,
            CreatedAt = now,
            NewsletterOptInAt = newsletter ? now : null
        };

        var result = await users.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            // A race with another registration of the same address must not reveal anything.
            if (result.Errors.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName"))
                return ServiceResult.Ok();

            return ServiceResult.Fail("invalid", "Oplysningerne kunne ikke bruges. Tjek dem og prøv igen.");
        }

        logger.LogInformation("Account created for user {UserId}", user.Id);
        await SendConfirmationAsync(user);
        return ServiceResult.Ok();
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        var decoded = DecodeToken(token);
        if (user is null || decoded is null) return false;

        var result = await users.ConfirmEmailAsync(user, decoded);
        if (result.Succeeded) logger.LogInformation("Email confirmed for user {UserId}", user.Id);
        return result.Succeeded;
    }

    public async Task ResendConfirmationAsync(string email)
    {
        email = email.Trim();
        var user = await users.FindByEmailAsync(email);
        if (user is null || user.EmailConfirmed || OnCooldown("confirm", email)) return;

        await SendConfirmationAsync(user);
    }

    // ---------- Login / refresh / logout ----------

    public async Task<ClaimsPrincipal?> LoginAsync(string email, string password, string clientIp)
    {
        var user = await users.FindByEmailAsync(email.Trim());

        if (user is null)
        {
            BurnHash(password); // same work as a real check, so timing does not reveal unknown addresses
            logger.LogWarning("Login failed (unknown account) from {ClientIp}", clientIp);
            return null;
        }

        var result = await signIn.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsNotAllowed || result.IsLockedOut) BurnHash(password);

            logger.LogWarning(
                "Login failed for user {UserId} (locked out: {LockedOut}) from {ClientIp}",
                user.Id, result.IsLockedOut, clientIp);
            return null;
        }

        var principal = await signIn.CreateUserPrincipalAsync(user);
        logger.LogInformation("Login succeeded for user {UserId} from {ClientIp}", user.Id, clientIp);

        if (await users.IsInRoleAsync(user, AppRoles.Admin))
            await emails.SendAdminLoginNoticeAsync(user, clientIp, time.GetUtcNow());

        return principal;
    }

    public async Task<ClaimsPrincipal?> RefreshAsync(string refreshToken)
    {
        var protector = bearer.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var ticket = protector.Unprotect(refreshToken);

        if (ticket?.Properties.ExpiresUtc is not { } expires || time.GetUtcNow() >= expires)
            return null;

        // Fails if the password changed, "log out everywhere" was used, or the stamp changed.
        var user = await signIn.ValidateSecurityStampAsync(ticket.Principal);
        if (user is null) return null;

        if (!await signIn.CanSignInAsync(user) || await users.IsLockedOutAsync(user))
            return null;

        return await signIn.CreateUserPrincipalAsync(user);
    }

    public async Task LogoutAllAsync(Guid userId)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        if (user is null) return;

        await users.UpdateSecurityStampAsync(user);
        logger.LogInformation("All sessions revoked for user {UserId}", user.Id);
    }

    // ---------- Forgot / reset ----------

    public async Task ForgotPasswordAsync(string email)
    {
        email = email.Trim();
        var user = await users.FindByEmailAsync(email);
        if (user is null || !user.EmailConfirmed || OnCooldown("reset", email)) return;

        var token = await users.GeneratePasswordResetTokenAsync(user);
        await emails.SendPasswordResetAsync(user, BuildLink("/nulstil-adgangskode", user.Id, token));
    }

    public async Task<ServiceResult> ResetPasswordAsync(Guid userId, string token, string newPassword)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        var decoded = DecodeToken(token);

        if (user is null || decoded is null)
        {
            BurnHash(newPassword);
            return ServiceResult.Fail("invalid_token", "Linket er ugyldigt eller udløbet.");
        }

        var result = await users.ResetPasswordAsync(user, decoded, newPassword);

        if (!result.Succeeded)
        {
            return result.Errors.Any(e => e.Code.StartsWith("Password", StringComparison.Ordinal))
                ? ServiceResult.Fail("weak_password", WeakPasswordMessage)
                : ServiceResult.Fail("invalid_token", "Linket er ugyldigt eller udløbet.");
        }

        // Proving control of the mailbox also clears a lockout. The reset itself already
        // changed the security stamp, so every old session dies at its next refresh.
        await users.SetLockoutEndDateAsync(user, null);
        await users.ResetAccessFailedCountAsync(user);

        logger.LogInformation("Password reset for user {UserId}", user.Id);
        return ServiceResult.Ok();
    }

    // ---------- Helpers ----------

    private async Task SendConfirmationAsync(AppUser user)
    {
        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        await emails.SendConfirmationAsync(user, BuildLink("/auth/bekraeft", user.Id, token));
    }

    private async Task<ServiceResult> ValidatePasswordAsync(string password)
    {
        foreach (var validator in users.PasswordValidators)
        {
            var result = await validator.ValidateAsync(users, DummyUser, password);
            if (!result.Succeeded) return ServiceResult.Fail("weak_password", WeakPasswordMessage);
        }

        return ServiceResult.Ok();
    }

    // True if a mail of this kind was sent to this address in the last couple of minutes.
    // Otherwise starts the cool-down and returns false. Stops people from mail-bombing an address.
    private bool OnCooldown(string kind, string email)
    {
        var key = $"mail:{kind}:{email.Trim().ToLowerInvariant()}";
        if (cache.TryGetValue(key, out _)) return true;

        cache.Set(key, true, MailCooldown);
        return false;
    }

    private void BurnHash(string password) =>
        users.PasswordHasher.HashPassword(DummyUser, password);

    private string BuildLink(string path, Guid userId, string token)
    {
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return $"{options.Value.FrontendBaseUrl.TrimEnd('/')}{path}?userId={userId}&token={encoded}";
    }

    private static string? DecodeToken(string token)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
