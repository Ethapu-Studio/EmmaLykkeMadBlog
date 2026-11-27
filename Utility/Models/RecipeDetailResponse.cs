namespace EmmaLykkeMadBlog.Utility.Models;

// Everything the recipe page needs, in one call.
public record RecipeDetailResponse(
    string Slug,
    string Title,
    string ShortDescription,
    string? Introduction,
    string CategoryName,
    string CategorySlug,
    string? ImagePath,
    string? ImageAlt,
    int PrepMinutes,
    int CookMinutes,
    int TotalMinutes,
    int Servings,
    DateTime? PublishedAt,
    IReadOnlyList<RecipeIngredientGroupResponse> IngredientGroups,
    IReadOnlyList<RecipeStepResponse> Steps,
    IReadOnlyList<string> Tags,
    RecipeNutritionResponse Nutrition);