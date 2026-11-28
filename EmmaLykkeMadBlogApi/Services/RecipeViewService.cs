using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EmmaLykkeMadBlog.Services;

public class RecipeViewService(
    IRecipeRepository recipes, IRecipeViewBuffer buffer, IMemoryCache cache, TimeProvider clock) : IRecipeViewService
{
    private static readonly TimeSpan IdCacheTime = TimeSpan.FromMinutes(5);

    public async Task<bool> RegisterViewAsync(string slug, string visitorId, CancellationToken ct)
    {
        var recipeId = await GetPublishedIdAsync(slug, ct);
        if (recipeId is null) return false;

        buffer.Add(recipeId.Value, visitorId, DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime));
        return true;
    }

    // Found ids are cached briefly so a view does not cost a database call. Misses are never cached.
    private async Task<int?> GetPublishedIdAsync(string slug, CancellationToken ct)
    {
        var key = $"recipes:id:{slug}";
        if (cache.TryGetValue(key, out int cached)) return cached;

        var id = await recipes.GetPublishedIdBySlugAsync(slug, ct);
        if (id is null) return null;

        cache.Set(key, id.Value, IdCacheTime);
        return id.Value;
    }
}