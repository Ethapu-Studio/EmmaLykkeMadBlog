namespace EmmaLykkeMadBlog.DTOs;

public sealed class AuthResetPasswordForm
{
    public Guid UserId { get; set; }
    public string? Token { get; set; }
    public string? NewPassword { get; set; }
}
