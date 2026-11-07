using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

[Index(nameof(RecipeId), nameof(StepNumber), IsUnique = true)]
public class RecipeStep
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Recipe))]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int StepNumber { get; set; }

    [Required]
    public string Instruction { get; set; } = string.Empty;
}