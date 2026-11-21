using System.Globalization;
using EmmaLykkeMadBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EmmaLykkeMadBlog.Services;

public class AuthCookieEvents(IAuthApiClient api, ILogger<AuthCookieEvents> logger) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var refreshToken = context.Properties.GetTokenValue("refresh_token");
        var expiresText = context.Properties.GetTokenValue("expires_at");

        if (string.IsNullOrEmpty(refreshToken) ||
            !DateTimeOffset.TryParse(expiresText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresAt))
        {
            await RejectAsync(context);
            return;
        }

        if (expiresAt - DateTimeOffset.UtcNow > TimeSpan.FromSeconds(60)) return;

        try
        {
            var tokens = await api.RefreshAsync(refreshToken, context.HttpContext.RequestAborted);
            if (tokens is null)
            {
                await RejectAsync(context);
                return;
            }

            context.Properties.UpdateTokenValue("access_token", tokens.AccessToken);
            context.Properties.UpdateTokenValue("refresh_token", tokens.RefreshToken);
            context.Properties.UpdateTokenValue("expires_at", tokens.ExpiresAt.ToString("o", CultureInfo.InvariantCulture));
            context.ShouldRenew = true;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // The API is briefly unavailable: keep the session and try again on the next request.
            logger.LogWarning(ex, "Could not refresh the session because the API is unavailable");
        }
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.Redirect("/?auth=required");
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.Redirect("/");
        return Task.CompletedTask;
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
