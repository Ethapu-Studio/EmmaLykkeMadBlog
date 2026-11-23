namespace EmmaLykkeMadBlog.DTOs;

public sealed class AuthForgotPasswordForm
{
    public string? Email { get; set; }
    public string? ReturnUrl { get; set; }
}
