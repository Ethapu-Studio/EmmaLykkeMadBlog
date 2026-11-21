using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

public class AuthConfirmEmailRequest
{
    public Guid UserId { get; set; }

    [Required, StringLength(2048)]
    public string Token { get; set; } = string.Empty;
}
