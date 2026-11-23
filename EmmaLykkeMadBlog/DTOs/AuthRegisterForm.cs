namespace EmmaLykkeMadBlog.DTOs;

public sealed class AuthRegisterForm
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool Newsletter { get; set; }
    public string? ReturnUrl { get; set; }
}
