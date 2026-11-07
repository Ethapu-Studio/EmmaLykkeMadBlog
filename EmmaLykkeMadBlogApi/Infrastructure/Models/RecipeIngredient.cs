using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    [Table("recipe_ingredient")]
    public class RecipeIngredient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // foreign key to Recipe.Id
        [Required]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Precision(8, 2)]
        [Column("amount")]
        public decimal? Amount { get; set; }

        [MaxLength(50)]
        [Column("unit")]
        public string? Unit { get; set; }

        [MaxLength(200)]
        [Column("note")]
        public string? Note { get; set; }

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; }
    }
}