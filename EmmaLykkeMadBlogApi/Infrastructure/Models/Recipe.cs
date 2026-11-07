using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    [Table("recipe")]
    [Index(nameof(Slug), IsUnique = true)]
    public class Recipe
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("slug")]
        public string Slug { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        [Column("short_description")]
        public string ShortDescription { get; set; } = null!;

        [Column("introduction")]
        public string? Introduction { get; set; }

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Required]
        [Column("prep_time_minutes")]
        public int PrepTimeMinutes { get; set; }

        [Required]
        [Column("cook_time_minutes")]
        public int CookTimeMinutes { get; set; }

        [Required]
        [Column("servings")]
        public int Servings { get; set; }

        // Nutrition per serving
        [Precision(8, 2)]
        [Column("calories")]
        public decimal? Calories { get; set; }

        [Precision(8, 2)]
        [Column("protein_g")]
        public decimal? ProteinG { get; set; }

        [Precision(8, 2)]
        [Column("carbs_g")]
        public decimal? CarbsG { get; set; }

        [Precision(8, 2)]
        [Column("fat_g")]
        public decimal? FatG { get; set; }

        [Precision(8, 2)]
        [Column("fiber_g")]
        public decimal? FiberG { get; set; }

        // foreign key to Category.Id
        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [Required]
        [Column("is_published")]
        public bool IsPublished { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public List<RecipeIngredient> Ingredients { get; set; } = new();
        public List<RecipeStep> Steps { get; set; } = new();
        public List<RecipeTag> RecipeTags { get; set; } = new();
    }
}