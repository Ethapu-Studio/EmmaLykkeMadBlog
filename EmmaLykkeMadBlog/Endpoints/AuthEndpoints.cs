using System.Globalization;
using System.Security.Claims;
using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmmaLykkeMadBlog.Endpoints;

// What the browser's forms post to. Every POST is antiforgery-protected (UseAntiforgery),
// rate limited per visitor address (the "auth" policy), and answers with a redirect.
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth").RequireRateLimiting("auth");

        auth.MapPost("/login", Login);
        auth.MapPost("/register", Register);
        auth.MapPost("/forgot", Forgot);
        auth.MapPost("/reset", Reset);
        auth.MapPost("/logout", Logout);
        auth.MapGet("/bekraeft", Confirm);
    }

    // ---------- Login ----------

    private static async Task<IResult> Login(
        [FromForm] AuthLoginForm form, HttpContext http, IAuthApiClient api, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrEmpty(form.Password))
            return Back(form.ReturnUrl, "login-failed");

        try
        {
            var tokens = await api.LoginAsync(
                new AuthLoginRequest { Email = form.Email.Trim(), Password = form.Password },
                ClientIp(http), ct);

            var me = tokens is null ? null : await api.GetMeAsync(tokens.AccessToken, ct);
            if (tokens is null || me is null) return Back(form.ReturnUrl, "login-failed");

            var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, me.Id.ToString()),
                    new Claim(ClaimTypes.Name, me.DisplayName)
                ],
                CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            // The tokens live only inside the encrypted login cookie, never in browser JavaScript.
            properties.StoreTokens(
            [
                new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken },
                new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = tokens.ExpiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            ]);

            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            return Results.LocalRedirect(SafePath(form.ReturnUrl));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Back(form.ReturnUrl, "unavailable");
        }
    }

    // ---------- Register ----------

    private static async Task<IResult> Register(
        [FromForm] AuthRegisterForm form, IAuthApiClient api, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Name) ||
            string.IsNullOrWhiteSpace(form.Email) ||
            string.IsNullOrEmpty(form.Password))
            return Back(form.ReturnUrl, "register-invalid");

        try
        {
            var result = await api.RegisterAsync(
                new AuthRegisterRequest
                {
                    Name = form.Name.Trim(),
                    Email = form.Email.Trim(),
                    Password = form.Password,
                    Newsletter = form.Newsletter
                }, ct);

            return Back(form.ReturnUrl, result.Ok ? "register-sent" : "register-invalid");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Back(form.ReturnUrl, "unavailable");
        }
    }

    // ---------- Forgot / reset ----------

    private static async Task<IResult> Forgot(
        [FromForm] AuthForgotPasswordForm form, IAuthApiClient api, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Email))
            return Back(form.ReturnUrl, "forgot-sent");

        try
        {
            await api.ForgotPasswordAsync(new AuthForgotPasswordRequest { Email = form.Email.Trim() }, ct);
            return Back(form.ReturnUrl, "forgot-sent"); // same answer whether or not the address exists
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Back(form.ReturnUrl, "unavailable");
        }
    }

    private static async Task<IResult> Reset(
        [FromForm] AuthResetPasswordForm form, HttpContext http, IAuthApiClient api, CancellationToken ct)
    {
        http.Response.Headers["Referrer-Policy"] = "no-referrer";

        if (string.IsNullOrWhiteSpace(form.Token) || string.IsNullOrEmpty(form.NewPassword))
            return Results.LocalRedirect("/?auth=reset-failed");

        try
        {
            var result = await api.ResetPasswordAsync(
                new AuthResetPasswordRequest
                {
                    UserId = form.UserId,
                    Token = form.Token,
                    NewPassword = form.NewPassword
                }, ct);

            if (result.Ok) return Results.LocalRedirect("/?auth=reset-ok");

            return result.Code == "weak_password"
                ? Results.LocalRedirect(
                    $"/nulstil-adgangskode?userId={form.UserId}&token={Uri.EscapeDataString(form.Token)}&error=weak")
                : Results.LocalRedirect("/?auth=reset-failed");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Results.LocalRedirect("/?auth=unavailable");
        }
    }

    // ---------- Confirm email (the link in the email) ----------

    private static async Task<IResult> Confirm(
        Guid? userId, string? token, HttpContext http, IAuthApiClient api, CancellationToken ct)
    {
        http.Response.Headers["Referrer-Policy"] = "no-referrer";

        if (userId is null || string.IsNullOrWhiteSpace(token))
            return Results.LocalRedirect("/?auth=confirm-failed");

        try
        {
            var result = await api.ConfirmEmailAsync(
                new AuthConfirmEmailRequest { UserId = userId.Value, Token = token }, ct);

            return Results.LocalRedirect(result.Ok ? "/?auth=confirmed" : "/?auth=confirm-failed");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Results.LocalRedirect("/?auth=unavailable");
        }
    }

    // ---------- Logout ----------

    // The unused form parameter makes the antiforgery check apply to this POST too.
    private static async Task<IResult> Logout(HttpContext http, IFormCollection form)
    {
        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.LocalRedirect("/");
    }

    // ---------- Helpers ----------

    private static IResult Back(string? returnUrl, string code) =>
        Results.LocalRedirect($"{SafePath(returnUrl)}?auth={code}");

    // Only plain local paths are accepted. Anything else becomes "/" (no open redirects).
    private static string SafePath(string? url) =>
        !string.IsNullOrEmpty(url)
        && url.Length <= 200
        && url[0] == '/'
        && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))
        && !url.Contains('?')
        && !url.Contains('#')
            ? url
            : "/";

    private static string ClientIp(HttpContext http) =>
        http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
