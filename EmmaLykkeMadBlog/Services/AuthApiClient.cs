using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services;

// Talks to the API from the frontend's SERVER. The browser never calls the API
// and never sees a token.
public class AuthApiClient(HttpClient http) : IAuthApiClient
{
    public async Task<ApiResult> RegisterAsync(AuthRegisterRequest request, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("api/auth/register", request, ct);
        return await ToResultAsync(response, ct);
    }

    public async Task<ApiResult> ConfirmEmailAsync(AuthConfirmEmailRequest request, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("api/auth/confirm-email", request, ct);
        return await ToResultAsync(response, ct);
    }

    public async Task<ApiResult> ForgotPasswordAsync(AuthForgotPasswordRequest request, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("api/auth/forgot-password", request, ct);
        return await ToResultAsync(response, ct);
    }

    public async Task<ApiResult> ResetPasswordAsync(AuthResetPasswordRequest request, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("api/auth/reset-password", request, ct);
        return await ToResultAsync(response, ct);
    }

    // Returns null for a failed login. Throws HttpRequestException if the API cannot be reached.
    public async Task<TokenSet?> LoginAsync(AuthLoginRequest request, string clientIp, CancellationToken ct)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/auth/login")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Client-Ip", clientIp);

        using var response = await http.SendAsync(message, ct);
        return response.IsSuccessStatusCode ? await ReadTokensAsync(response, ct) : null;
    }

    // Returns null when the refresh token is no longer valid.
    // Throws HttpRequestException on other failures, so a short API outage does not log everyone out.
    public async Task<TokenSet?> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync(
            "api/auth/refresh", new AuthRefreshRequest { RefreshToken = refreshToken }, ct);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
            return null;

        response.EnsureSuccessStatusCode();
        return await ReadTokensAsync(response, ct);
    }

    public async Task<AccountMeResponse?> GetMeAsync(string accessToken, CancellationToken ct)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/account/me");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await http.SendAsync(message, ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AccountMeResponse>(ct)
            : null;
    }

    private static async Task<TokenSet?> ReadTokensAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadFromJsonAsync<AuthTokenResponse>(ct);
        return body is null
            ? null
            : new TokenSet(body.AccessToken, body.RefreshToken, DateTimeOffset.UtcNow.AddSeconds(body.ExpiresIn));
    }

    private static async Task<ApiResult> ToResultAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return new ApiResult(true);

        string? code = null;
        try
        {
            code = (await response.Content.ReadFromJsonAsync<AuthErrorResponse>(ct))?.Code;
        }
        catch (JsonException) { }
        catch (NotSupportedException) { }

        return new ApiResult(false, code ?? ((int)response.StatusCode).ToString());
    }
}
