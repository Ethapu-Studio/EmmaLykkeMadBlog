using System.Net;
using System.Security.Claims;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmmaLykkeMadBlog.Controllers;

// Called only by the Blazor frontend's server, never by the browser.
// Controllers take input, validate it and call the service. No logic lives here.
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        var result = await auth.RegisterAsync(request.Name, request.Email, request.Password, request.Newsletter);

        // 202 for every outcome that does not reveal anything about existing accounts.
        return result.Succeeded
            ? Accepted()
            : BadRequest(new AuthErrorResponse(result.Code ?? "invalid", result.Errors));
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(AuthConfirmEmailRequest request) =>
        await auth.ConfirmEmailAsync(request.UserId, request.Token)
            ? NoContent()
            : BadRequest(new AuthErrorResponse("invalid_token"));

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(AuthResendConfirmationRequest request)
    {
        await auth.ResendConfirmationAsync(request.Email);
        return Accepted();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthLoginRequest request)
    {
        var principal = await auth.LoginAsync(request.Email, request.Password, GetClientIp());

        // One answer for wrong password, unknown address, unconfirmed and locked out.
        return principal is null
            ? Unauthorized(new AuthErrorResponse("invalid_credentials"))
            : SignIn(principal, IdentityConstants.BearerScheme);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(AuthRefreshRequest request)
    {
        var principal = await auth.RefreshAsync(request.RefreshToken);

        return principal is null
            ? Unauthorized(new AuthErrorResponse("invalid_refresh_token"))
            : SignIn(principal, IdentityConstants.BearerScheme);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(AuthForgotPasswordRequest request)
    {
        await auth.ForgotPasswordAsync(request.Email);
        return Accepted();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(AuthResetPasswordRequest request)
    {
        var result = await auth.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);

        return result.Succeeded
            ? NoContent()
            : BadRequest(new AuthErrorResponse(result.Code ?? "invalid_token", result.Errors));
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        await auth.LogoutAllAsync(userId);
        return NoContent();
    }

    // The frontend forwards the visitor's address. Used for logs only, never for decisions.
    private string GetClientIp() =>
        IPAddress.TryParse(Request.Headers["X-Client-Ip"].FirstOrDefault(), out var ip)
            ? ip.ToString()
            : "unknown";
}
