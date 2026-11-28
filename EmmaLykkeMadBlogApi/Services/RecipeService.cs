using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.Extensions.Caching.Memory;
using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Services;

public class RecipeService(IRecipeRepository recipes, IMemoryCache cache, TimeProvider clock) : IRecipeService
{
    private const string HomeCacheKey = "recipes:home";
    private static readonly TimeSpan HomeCacheTime = TimeSpan.FromSeconds(60);
    private const int PopularCount = 3;
    private const int LatestCount = 8;
    private const int ListPageSize = 12;
    private const int PopularWindowDays = 30;
    private static readonly TimeSpan DetailCacheTime = TimeSpan.FromSeconds(60);

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
    
    // The slug is validated by the controller before it gets here.
    public async Task<RecipeDetailResponse?> GetDetailAsync(string slug, CancellationToken ct)
    {
        var key = $"recipes:detail:{slug}";
        if (cache.TryGetValue(key, out RecipeDetailResponse? cached) && cached is not null)
            return cached;

        var recipe = await recipes.GetPublishedBySlugAsync(slug, ct);
        if (recipe is null) return null; // not cached, so random addresses cannot fill the cache

        var detail = ToDetail(recipe);
        cache.Set(key, detail, DetailCacheTime);
        return detail;
    }

    // Lists are not cached: the search text is free input, so a cache would grow without limit.
    public async Task<RecipeListResponse> GetListAsync(RecipeListRequest request, CancellationToken ct)
    {
        var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();
        var category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category;

        // Relevance only makes sense with a search text. Otherwise the newest come first.
        var sort = request.Sort ?? (search is null ? RecipeSort.Newest : RecipeSort.Relevance);
        if (sort == RecipeSort.Relevance && search is null) sort = RecipeSort.Newest;

        var total = await recipes.CountPublishedAsync(search, category, ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)ListPageSize));
        var page = Math.Min(request.Page, totalPages);

        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        var items = await recipes.GetPublishedPageAsync(
            sort, search, category, today.AddDays(-(PopularWindowDays - 1)), (page - 1) * ListPageSize, ListPageSize, ct);

        var categories = await recipes.GetCategoriesWithCountsAsync(ct);

        return new RecipeListResponse(items, total, page, ListPageSize, totalPages, categories);
    }

    private static RecipeDetailResponse ToDetail(Recipe r) => new(
        r.Slug, r.Title, r.ShortDescription, r.Introduction,
        r.Category.Name, r.Category.Slug,
        r.ImagePath, r.ImageAlt,
        r.PrepTimeMinutes, r.CookTimeMinutes, r.PrepTimeMinutes + r.CookTimeMinutes, r.Servings,
        r.PublishedAt,
        r.Ingredients
            .OrderBy(i => i.SortOrder)
            .GroupBy(i => i.GroupName)
            .Select(g => new RecipeIngredientGroupResponse(
                g.Key,
                g.Select(i => new RecipeIngredientResponse(i.Name, i.Amount, i.Unit, i.Note)).ToList()))
            .ToList(),
        r.Steps
            .OrderBy(s => s.StepNumber)
            .Select(s => new RecipeStepResponse(s.StepNumber, s.Instruction))
            .ToList(),
        r.RecipeTags.Select(t => t.Tag.Name).OrderBy(n => n).ToList(),
        new RecipeNutritionResponse(
            r.Calories, r.ProteinG, r.CarbsG, r.FatG, r.FiberG, r.SugarG, r.SaltG, r.SaturatedFatG));
}
