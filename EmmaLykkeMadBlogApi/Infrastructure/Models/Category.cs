using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Slug), IsUnique = true)]
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    public List<Recipe> Recipes { get; set; } = new();
}