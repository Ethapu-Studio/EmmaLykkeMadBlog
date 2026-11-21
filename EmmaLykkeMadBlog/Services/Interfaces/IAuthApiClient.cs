using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IAuthApiClient
{
    Task<ApiResult> RegisterAsync(AuthRegisterRequest request, CancellationToken ct);
    Task<ApiResult> ConfirmEmailAsync(AuthConfirmEmailRequest request, CancellationToken ct);
    Task<ApiResult> ForgotPasswordAsync(AuthForgotPasswordRequest request, CancellationToken ct);
    Task<ApiResult> ResetPasswordAsync(AuthResetPasswordRequest request, CancellationToken ct);
    Task<TokenSet?> LoginAsync(AuthLoginRequest request, string clientIp, CancellationToken ct);
    Task<TokenSet?> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task<AccountMeResponse?> GetMeAsync(string accessToken, CancellationToken ct);
}
