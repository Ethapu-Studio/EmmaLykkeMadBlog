using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Repositories.Interfaces;

public interface IPopularRecipeRepository
{
    // The best published recipes since the given day, most unique visitors first. One query.
    Task<IReadOnlyList<RecipeRanking>> GetRankingAsync(DateOnly since, int count, CancellationToken ct);

    // The current list, tracked so changes to it are saved. One query.
    Task<List<PopularRecipe>> GetCurrentAsync(CancellationToken ct);

    // In-memory only: nothing reaches the database until SaveChangesAsync.
    void Add(PopularRecipe row);
    void Remove(PopularRecipe row);

    // Saves all added, changed and removed rows together. One call.
    Task SaveChangesAsync(CancellationToken ct);
}