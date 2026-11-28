using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetPublishedBySlugAsync(string slug, CancellationToken ct);
    Task<IReadOnlyList<RecipeCardResponse>> GetLatestAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<RecipeCardResponse>> GetPopularAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<RecipeCategoryResponse>> GetCategoriesWithCountsAsync(CancellationToken ct);
    Task<int?> GetPublishedIdBySlugAsync(string slug, CancellationToken ct);
    Task<int> CountPublishedAsync(string? search, string? categorySlug, CancellationToken ct);
    Task<IReadOnlyList<RecipeCardResponse>> GetPublishedPageAsync(
        RecipeSort sort, string? search, string? categorySlug, DateOnly popularSince, int skip, int take, CancellationToken ct);
}
