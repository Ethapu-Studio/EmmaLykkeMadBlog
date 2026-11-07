using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

[Index(nameof(Slug), IsUnique = true)]
public class Recipe
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    public string? Introduction { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }

    // Nutrition per serving
    [Precision(8, 2)] public decimal? Calories { get; set; }
    [Precision(8, 2)] public decimal? ProteinG { get; set; }
    [Precision(8, 2)] public decimal? CarbsG { get; set; }
    [Precision(8, 2)] public decimal? FatG { get; set; }
    [Precision(8, 2)] public decimal? FiberG { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public List<RecipeStep> Steps { get; set; } = new();
    public List<RecipeTag> RecipeTags { get; set; } = new();
}