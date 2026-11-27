namespace EmmaLykkeMadBlog.Utility.Models;

// Nutrition per serving. Any value can be missing.
public record RecipeNutritionResponse(
    decimal? Calories,
    decimal? ProteinG,
    decimal? CarbsG,
    decimal? FatG,
    decimal? FiberG,
    decimal? SugarG,
    decimal? SaltG,
    decimal? SaturatedFatG);