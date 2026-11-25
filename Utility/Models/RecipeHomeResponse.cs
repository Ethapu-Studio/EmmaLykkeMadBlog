namespace EmmaLykkeMadBlog.Utility.Models;

// Everything the front page needs, in one call.
public record RecipeHomeResponse(
    IReadOnlyList<RecipeCardResponse> Popular,
    IReadOnlyList<RecipeCardResponse> Latest,
    IReadOnlyList<RecipeCategoryResponse> Categories);
