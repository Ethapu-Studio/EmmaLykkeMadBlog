using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<IReadOnlyList<RecipeCardResponse>> GetLatestAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<RecipeCardResponse>> GetPopularAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<RecipeCategoryResponse>> GetCategoriesWithCountsAsync(CancellationToken ct);
}
