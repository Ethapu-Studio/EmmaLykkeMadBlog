using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

public class AuthRefreshRequest
{
    [Required, StringLength(4096)]
    public string RefreshToken { get; set; } = string.Empty;
}
