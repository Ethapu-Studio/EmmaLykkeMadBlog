using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EmmaLykkeMadBlog.Services;

public class RecipeService(IRecipeRepository recipes, IMemoryCache cache) : IRecipeService
{
    private const string HomeCacheKey = "recipes:home";
    private static readonly TimeSpan HomeCacheTime = TimeSpan.FromSeconds(60);
    private const int PopularCount = 3;
    private const int LatestCount = 8;

    public async Task<RecipeHomeResponse> GetHomeAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(HomeCacheKey, out RecipeHomeResponse? cached) && cached is not null)
            return cached;

        var latest = await recipes.GetLatestAsync(LatestCount, ct);
        var popular = (await recipes.GetPopularAsync(PopularCount, ct)).ToList();

        // Until enough recipes have views, the popular box is filled up with the newest recipes.
        foreach (var recipe in latest)
        {
            if (popular.Count >= PopularCount) break;
            if (popular.All(p => p.Slug != recipe.Slug)) popular.Add(recipe);
        }

        var categories = await recipes.GetCategoriesWithCountsAsync(ct);
        var home = new RecipeHomeResponse(popular, latest, categories);

        cache.Set(HomeCacheKey, home, HomeCacheTime);
        return home;
    }
}
