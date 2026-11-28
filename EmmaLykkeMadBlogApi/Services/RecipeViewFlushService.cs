using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

// Saves the in-memory view counts to the database every few minutes and once more at shutdown.
public class RecipeViewFlushService(
    IRecipeViewBuffer buffer,
    IServiceScopeFactory scopes,
    TimeProvider clock,
    ILogger<RecipeViewFlushService> logger) : BackgroundService
{
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMinutes(3);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(FlushInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
                await FlushAsync();
        }
        catch (OperationCanceledException)
        {
            // The service is shutting down.
        }

        await FlushAsync();
    }

    private async Task FlushAsync()
    {
        var counts = buffer.Drain(DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime));
        if (counts.Count == 0) return;

        try
        {
            using var scope = scopes.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRecipeViewRepository>();
            await repository.AddDailyViewsAsync(counts, CancellationToken.None);

            logger.LogInformation("Saved recipe views for {Count} recipe-days", counts.Count);
        }
        catch (Exception ex)
        {
            buffer.Restore(counts); // tried again at the next flush
            logger.LogError("Saving recipe views failed: {Error}", ex.GetType().Name);
        }
    }
}