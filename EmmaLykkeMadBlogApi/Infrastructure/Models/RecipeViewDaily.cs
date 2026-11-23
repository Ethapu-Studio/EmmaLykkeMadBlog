using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    // One row per recipe per day. Counts only: no visitor data is ever stored.
    [Table("recipe_view_daily")]
    [PrimaryKey(nameof(RecipeId), nameof(ViewDate))]
    public class RecipeViewDaily
    {
        // foreign key to Recipe.Id
        [Required]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        [Required]
        [Column("view_date", TypeName = "date")]
        public DateOnly ViewDate { get; set; }

        [Required]
        [Column("views")]
        public int Views { get; set; }

        [Required]
        [Column("unique_visitors")]
        public int UniqueVisitors { get; set; }
    }
}
