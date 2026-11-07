using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    [Table("recipe_tag")]
    [PrimaryKey(nameof(RecipeId), nameof(TagId))]
    public class RecipeTag
    {
        // foreign key to Recipe.Id
        [Required]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        // foreign key to Tag.Id
        [Required]
        [Column("tag_id")]
        public int TagId { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; } = null!;
    }
}