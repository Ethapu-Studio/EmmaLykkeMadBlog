using EmmaLykkeMadBlog.Infrastructure;
using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Repositories;

// Read-only queries. Only published recipes are ever returned.
public class RecipeRepository(EmmaLykkeMadDbContext db) : IRecipeRepository
{
    public async Task<IReadOnlyList<RecipeCardResponse>> GetLatestAsync(int count, CancellationToken ct) =>
        await db.Recipe
            .AsNoTracking()
            .Where(r => r.IsPublished)
            .OrderByDescending(r => r.PublishedAt)
            .Take(count)
            .Select(r => new RecipeCardResponse(
                r.Slug, r.Title, r.ShortDescription,
                r.Category.Name, r.Category.Slug,
                r.PrepTimeMinutes + r.CookTimeMinutes,
                r.ImagePath, r.ImageAlt))
            .ToListAsync(ct);

    // The ready-made list the ranking job writes (see popular_recipe).
    public async Task<IReadOnlyList<RecipeCardResponse>> GetPopularAsync(int count, CancellationToken ct) =>
        await db.PopularRecipe
            .AsNoTracking()
            .Where(p => p.Recipe.IsPublished)
            .OrderBy(p => p.Rank)
            .Take(count)
            .Select(p => new RecipeCardResponse(
                p.Recipe.Slug, p.Recipe.Title, p.Recipe.ShortDescription,
                p.Recipe.Category.Name, p.Recipe.Category.Slug,
                p.Recipe.PrepTimeMinutes + p.Recipe.CookTimeMinutes,
                p.Recipe.ImagePath, p.Recipe.ImageAlt))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RecipeCategoryResponse>> GetCategoriesWithCountsAsync(CancellationToken ct) =>
        await db.Category
            .AsNoTracking()
            .Where(c => c.Recipes.Any(r => r.IsPublished))
            .OrderBy(c => c.Name)
            .Select(c => new RecipeCategoryResponse(c.Name, c.Slug, c.Recipes.Count(r => r.IsPublished)))
            .ToListAsync(ct);
    
    
    // One call. Split query so ingredients, steps and tags do not multiply each other's rows.
    public async Task<Recipe?> GetPublishedBySlugAsync(string slug, CancellationToken ct) =>
        await db.Recipe
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.Category)
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
            .FirstOrDefaultAsync(r => r.Slug == slug && r.IsPublished, ct);
    
    
    public async Task<int?> GetPublishedIdBySlugAsync(string slug, CancellationToken ct) =>
        await db.Recipe
            .AsNoTracking()
            .Where(r => r.Slug == slug && r.IsPublished)
            .Select(r => (int?)r.Id)
            .FirstOrDefaultAsync(ct);
    
    public async Task<int> CountPublishedAsync(string? search, string? categorySlug, CancellationToken ct) =>
        await Filter(search, categorySlug).CountAsync(ct);

    public async Task<IReadOnlyList<RecipeCardResponse>> GetPublishedPageAsync(
        RecipeSort sort, string? search, string? categorySlug, DateOnly popularSince, int skip, int take, CancellationToken ct) =>
        await Order(Filter(search, categorySlug), sort, search, popularSince)
            .Skip(skip)
            .Take(take)
            .Select(r => new RecipeCardResponse(
                r.Slug, r.Title, r.ShortDescription,
                r.Category.Name, r.Category.Slug,
                r.PrepTimeMinutes + r.CookTimeMinutes,
                r.ImagePath, r.ImageAlt))
            .ToListAsync(ct);

    private IQueryable<Recipe> Filter(string? search, string? categorySlug)
    {
        var query = db.Recipe.AsNoTracking().Where(r => r.IsPublished);

        if (!string.IsNullOrWhiteSpace(categorySlug))
            query = query.Where(r => r.Category.Slug == categorySlug);

        // Every word must match the title, the short description or an ingredient name.
        foreach (var word in SearchWords(search))
        {
            var pattern = $"%{EscapeLike(word)}%";
            query = query.Where(r =>
                EF.Functions.ILike(r.Title, pattern, "\\") ||
                EF.Functions.ILike(r.ShortDescription, pattern, "\\") ||
                r.Ingredients.Any(i => EF.Functions.ILike(i.Name, pattern, "\\")));
        }

        return query;
    }

    // The last sort key is always the id, so paging gives the same order every time.
    private IQueryable<Recipe> Order(IQueryable<Recipe> query, RecipeSort sort, string? search, DateOnly popularSince)
    {
        switch (sort)
        {
            case RecipeSort.Alphabetical:
                return query.OrderBy(r => r.Title).ThenBy(r => r.Id);

            case RecipeSort.Fastest:
                return query
                    .OrderBy(r => r.PrepTimeMinutes + r.CookTimeMinutes)
                    .ThenByDescending(r => r.PublishedAt)
                    .ThenBy(r => r.Id);

            case RecipeSort.Popular:
                return query
                    .OrderByDescending(r => db.RecipeViewDaily
                        .Where(v => v.RecipeId == r.Id && v.ViewDate >= popularSince)
                        .Sum(v => v.UniqueVisitors))
                    .ThenByDescending(r => r.PublishedAt)
                    .ThenBy(r => r.Id);

            case RecipeSort.Relevance when !string.IsNullOrWhiteSpace(search):
            {
                // Recipes with the whole search text in the title come first.
                var phrase = $"%{EscapeLike(search.Trim())}%";
                return query
                    .OrderByDescending(r => EF.Functions.ILike(r.Title, phrase, "\\"))
                    .ThenByDescending(r => r.PublishedAt)
                    .ThenBy(r => r.Id);
            }

            default:
                return query.OrderByDescending(r => r.PublishedAt).ThenBy(r => r.Id);
        }
    }

    private static string[] SearchWords(string? search) =>
        string.IsNullOrWhiteSpace(search)
            ? Array.Empty<string>()
            : search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Take(5).ToArray();

    // % and _ are wildcards in LIKE. In the search text they are plain characters.
    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
