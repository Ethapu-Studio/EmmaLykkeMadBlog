using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Infrastructure;
using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Repositories;

public class PopularRecipeRepository(EmmaLykkeMadDbContext db) : IPopularRecipeRepository
{
    // Ties are broken by total views and then by recipe id, so the same data always gives the same order.
    public async Task<IReadOnlyList<RecipeRanking>> GetRankingAsync(DateOnly since, int count, CancellationToken ct) =>
        await db.RecipeViewDaily
            .AsNoTracking()
            .Where(v => v.ViewDate >= since && v.Recipe.IsPublished)
            .GroupBy(v => v.RecipeId)
            .OrderByDescending(g => g.Sum(v => v.UniqueVisitors))
            .ThenByDescending(g => g.Sum(v => v.Views))
            .ThenBy(g => g.Key)
            .Take(count)
            .Select(g => new RecipeRanking(g.Key, g.Sum(v => v.UniqueVisitors)))
            .ToListAsync(ct);

    public async Task<List<PopularRecipe>> GetCurrentAsync(CancellationToken ct) =>
        await db.PopularRecipe.ToListAsync(ct);

    public void Add(PopularRecipe row) => db.PopularRecipe.Add(row);

    public void Remove(PopularRecipe row) => db.PopularRecipe.Remove(row);

    public async Task SaveChangesAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
}