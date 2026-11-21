using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

public class AuthLoginRequest
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string Password { get; set; } = string.Empty;
}
