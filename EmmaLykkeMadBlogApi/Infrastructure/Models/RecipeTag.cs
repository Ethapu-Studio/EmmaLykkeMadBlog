using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

[PrimaryKey(nameof(RecipeId), nameof(TagId))]
public class RecipeTag
{
    [ForeignKey(nameof(Recipe))]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    [ForeignKey(nameof(Tag))]
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}