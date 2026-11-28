namespace EmmaLykkeMadBlog.Utility.Models;

// One page of the recipe list, plus what the filter needs.
public record RecipeListResponse(
    IReadOnlyList<RecipeCardResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<RecipeCategoryResponse> Categories);