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
}
