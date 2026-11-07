using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

public class RecipeIngredient
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Recipe))]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Precision(8, 2)]
    public decimal? Amount { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [MaxLength(200)]
    public string? Note { get; set; }

    public int SortOrder { get; set; }
}