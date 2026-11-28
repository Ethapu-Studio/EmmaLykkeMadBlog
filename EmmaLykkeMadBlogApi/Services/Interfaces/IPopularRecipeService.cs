namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IPopularRecipeService
{
    // Recalculates the popular list from the last 30 days. Safe to run any number of times.
    Task RefreshAsync(CancellationToken ct);
}