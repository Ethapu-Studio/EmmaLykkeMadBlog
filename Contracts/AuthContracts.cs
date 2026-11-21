using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Contracts;

// Only these fields can ever be submitted. There is deliberately no role, id or flag
// in any request, so nothing can be mass-assigned.

public class RegisterRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string Password { get; set; } = string.Empty;

    public bool Newsletter { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string Password { get; set; } = string.Empty;
}

public class EmailRequest
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;
}

public class ConfirmEmailRequest
{
    public Guid UserId { get; set; }

    [Required, StringLength(2048)]
    public string Token { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public Guid UserId { get; set; }

    [Required, StringLength(2048)]
    public string Token { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string NewPassword { get; set; } = string.Empty;
}

public class RefreshRequest
{
    [Required, StringLength(4096)]
    public string RefreshToken { get; set; } = string.Empty;
}

public record MeResponse(Guid Id, string DisplayName);

public record ErrorResponse(string Code, string[]? Errors = null);