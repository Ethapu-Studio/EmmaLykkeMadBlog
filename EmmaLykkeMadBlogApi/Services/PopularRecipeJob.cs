using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

// Runs the popular-recipes calculation once at startup and then every 3 hours. All logic is in the service.
public class PopularRecipeJob(IServiceScopeFactory scopes, ILogger<PopularRecipeJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(3);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            do
            {
                await RunAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // The service is shutting down.
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IPopularRecipeService>();
            await service.RefreshAsync(ct);

            logger.LogInformation("Popular recipes refreshed");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError("Refreshing popular recipes failed: {Error}", ex.GetType().Name);
        }
    }
}