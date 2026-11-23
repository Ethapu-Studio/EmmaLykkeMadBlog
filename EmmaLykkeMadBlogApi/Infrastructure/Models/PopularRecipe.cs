using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    // The ready-made "popular recipes" list. A scheduled job rewrites it a few times a day,
    // so the front page never has to add up views itself.
    [Table("popular_recipe")]
    public class PopularRecipe
    {
        // foreign key to Recipe.Id, also the primary key: a recipe is on the list at most once
        [Key]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public Recipe Recipe { get; set; } = null!;

        [Required]
        [Column("rank")]
        public int Rank { get; set; }

        // Views in the last 30 days when the list was calculated
        [Required]
        [Column("score")]
        public int Score { get; set; }

        [Required]
        [Column("calculated_at")]
        public DateTime CalculatedAt { get; set; }
    }
}
