using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    [Table("recipe_step")]
    [Index(nameof(RecipeId), nameof(StepNumber), IsUnique = true)]
    public class RecipeStep
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
        [Column("step_number")]
        public int StepNumber { get; set; }

        [Required]
        [Column("instruction")]
        public string Instruction { get; set; } = null!;
    }
}