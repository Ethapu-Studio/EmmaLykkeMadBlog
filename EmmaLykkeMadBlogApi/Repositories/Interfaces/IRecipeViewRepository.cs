using EmmaLykkeMadBlog.DTOs;

namespace EmmaLykkeMadBlog.Repositories.Interfaces;

public interface IRecipeViewRepository
{
    // Adds the counts to the daily rows (creating them if needed). One call.
    Task AddDailyViewsAsync(IReadOnlyList<RecipeViewCount> counts, CancellationToken ct);
}