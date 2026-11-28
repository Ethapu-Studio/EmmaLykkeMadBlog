using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

public class RecipeViewRequest
{
    // An anonymous value that changes every day, made by the frontend server. Never an address or anything readable.
    [Required]
    [StringLength(64, MinimumLength = 16)]
    [RegularExpression("^[A-Za-z0-9_-]+$")]
    public string VisitorId { get; set; } = "";
}