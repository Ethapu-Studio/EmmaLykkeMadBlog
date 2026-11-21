using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

public class AuthResetPasswordRequest
{
    public Guid UserId { get; set; }

    [Required, StringLength(2048)]
    public string Token { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string NewPassword { get; set; } = string.Empty;
}
