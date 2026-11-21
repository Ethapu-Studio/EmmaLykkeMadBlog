using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IAccountEmailService
{
    Task SendConfirmationAsync(AppUser user, string link);
    Task SendPasswordResetAsync(AppUser user, string link);
    Task SendAlreadyRegisteredAsync(AppUser user);
    Task SendAdminLoginNoticeAsync(AppUser user, string clientIp, DateTimeOffset when);
}
