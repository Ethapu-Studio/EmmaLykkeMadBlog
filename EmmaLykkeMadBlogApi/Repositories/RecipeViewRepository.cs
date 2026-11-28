using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Infrastructure;
using EmmaLykkeMadBlog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Repositories;

public class RecipeViewRepository(EmmaLykkeMadDbContext db) : IRecipeViewRepository
{
    // Adds the counts to the daily rows, creating rows that do not exist yet. Recipes that no longer exist are skipped.
    // Done with EF Core only: one query reads the existing rows, then one SaveChanges writes everything together.
    public async Task AddDailyViewsAsync(IReadOnlyList<RecipeViewCount> counts, CancellationToken ct)
    {
        var ids = counts.Select(c => c.RecipeId).Distinct().ToList();
        var days = counts.Select(c => c.Day).Distinct().ToList();

        // Every existing recipe once, together with its daily rows for these days (none gives null).
        var rows = await (
            from recipe in db.Recipe.Where(r => ids.Contains(r.Id))
            join daily in db.RecipeViewDaily.Where(d => days.Contains(d.ViewDate))
                on recipe.Id equals daily.RecipeId into joined
            from daily in joined.DefaultIfEmpty()
            select new { recipe.Id, Daily = daily })
            .ToListAsync(ct);

        var knownRecipes = rows.Select(r => r.Id).ToHashSet();
        var existing = rows
            .Where(r => r.Daily is not null)
            .ToDictionary(r => (r.Id, r.Daily!.ViewDate), r => r.Daily!);

        foreach (var count in counts.Where(c => knownRecipes.Contains(c.RecipeId)))
        {
            if (existing.TryGetValue((count.RecipeId, count.Day), out var row))
            {
                row.Views += count.Views;
                row.UniqueVisitors += count.NewVisitors;
            }
            else
            {
                db.RecipeViewDaily.Add(new RecipeViewDaily
                {
                    RecipeId = count.RecipeId,
                    ViewDate = count.Day,
                    Views = count.Views,
                    UniqueVisitors = count.NewVisitors
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}