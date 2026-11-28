using System.ComponentModel.DataAnnotations;

namespace EmmaLykkeMadBlog.Utility.Models;

// Query string for the public recipe list. Invalid input is rejected with 400 before the service runs.
public class RecipeListRequest
{
    [StringLength(100)]
    public string? Search { get; set; }

    [StringLength(100)]
    [RegularExpression("^[a-z0-9-]+$")]
    public string? Category { get; set; }

    [EnumDataType(typeof(RecipeSort))]
    public RecipeSort? Sort { get; set; }

    [Range(1, 1000)]
    public int Page { get; set; } = 1;
}