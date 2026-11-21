using EmmaLykkeMadBlog.DTOs;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
