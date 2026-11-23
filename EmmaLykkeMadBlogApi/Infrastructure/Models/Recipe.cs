using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmmaLykkeMadBlog.Infrastructure.Models
{
    [Table("recipe")]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(IsPublished), nameof(PublishedAt))]
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

                // A relative path under the image root, e.g. "recipes/2026/11/abc.webp". Never a full address.
        [MaxLength(500)]
        [Column("image_path")]
        public string? ImagePath { get; set; }

        [MaxLength(200)]
        [Column("image_alt")]
        public string? ImageAlt { get; set; }

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
        [Column("sugar_g")]
        public decimal? SugarG { get; set; }

        [Precision(8, 2)]
        [Column("salt_g")]
        public decimal? SaltG { get; set; }

        [Precision(8, 2)]
        [Column("saturated_fat_g")]
        public decimal? SaturatedFatG { get; set; }

        // foreign key to Category.Id
        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [Required]
        [Column("is_published")]
        public bool IsPublished { get; set; }

        // Set when the recipe is first published. "Seneste opskrifter" sorts on this.
        [Column("published_at")]
        public DateTime? PublishedAt { get; set; }

        // Chosen by the admin for the favourites section (used later).
        [Required]
        [Column("is_favorite")]
        public bool IsFavorite { get; set; }

        [Column("favorite_sort_order")]
        public int? FavoriteSortOrder { get; set; }

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