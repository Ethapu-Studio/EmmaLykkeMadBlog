using System.Security.Claims;
using EmmaLykkeMadBlog.DTOs;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> RegisterAsync(string name, string email, string password, bool newsletter);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task ResendConfirmationAsync(string email);
    Task<ClaimsPrincipal?> LoginAsync(string email, string password, string clientIp);
    Task<ClaimsPrincipal?> RefreshAsync(string refreshToken);
    Task ForgotPasswordAsync(string email);
    Task<ServiceResult> ResetPasswordAsync(Guid userId, string token, string newPassword);
    Task LogoutAllAsync(Guid userId);
}
