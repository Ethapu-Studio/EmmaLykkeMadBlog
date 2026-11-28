using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

public class PopularRecipeService(IPopularRecipeRepository popular, TimeProvider clock) : IPopularRecipeService
{
    private const int WindowDays = 30;
    private const int ListSize = 10;

    // Makes the table match the new ranking: rows are updated in place, new ones added, outdated ones removed,
    // and everything is saved in one go. Running it twice gives the same table, and it is never empty in between.
    // With no views at all the table ends up empty, and the home page falls back to the newest recipes.
    public async Task RefreshAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var since = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-(WindowDays - 1));

        var ranking = await popular.GetRankingAsync(since, ListSize, ct);
        var current = await popular.GetCurrentAsync(ct);

        var currentById = current.ToDictionary(p => p.RecipeId);
        var wanted = ranking.Select(r => r.RecipeId).ToHashSet();

        foreach (var outdated in current.Where(p => !wanted.Contains(p.RecipeId)))
            popular.Remove(outdated);

        for (var index = 0; index < ranking.Count; index++)
        {
            var item = ranking[index];

            if (currentById.TryGetValue(item.RecipeId, out var row))
            {
                row.Rank = index + 1;
                row.Score = item.Score;
                row.CalculatedAt = now.UtcDateTime;
            }
            else
            {
                popular.Add(new PopularRecipe
                {
                    RecipeId = item.RecipeId,
                    Rank = index + 1,
                    Score = item.Score,
                    CalculatedAt = now.UtcDateTime
                });
            }
        }

        await popular.SaveChangesAsync(ct);
    }
}