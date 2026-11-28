using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

// Decides whether a page load counts, makes the anonymous visitor value and reports the view to the API.
public class RecipeViewTracker(IRecipeApiClient api, IVisitorIdService visitors) : IRecipeViewTracker
{
    private static readonly TimeSpan CallLimit = TimeSpan.FromSeconds(2);

    // Simple filter for well-known robots. Extend here when needed.
    private static readonly string[] BotWords =
    [
        "bot", "crawl", "spider", "slurp", "preview", "headless", "monitor",
        "facebookexternalhit", "curl/", "wget/", "python-requests", "go-http-client", "okhttp"
    ];

    public async Task TrackAsync(HttpContext http, string slug, CancellationToken ct)
    {
        if (!HttpMethods.IsGet(http.Request.Method) || IsPrefetch(http.Request)) return;

        var userAgent = http.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent) || IsBot(userAgent)) return;

        var address = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var visitorId = visitors.Create(address, userAgent);

        using var limit = CancellationTokenSource.CreateLinkedTokenSource(ct);
        limit.CancelAfter(CallLimit);

        try
        {
            await api.TrackViewAsync(slug, visitorId, limit.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // Too slow: the view is skipped, the page does not wait.
        }
    }

    private static bool IsBot(string userAgent) =>
        BotWords.Any(word => userAgent.Contains(word, StringComparison.OrdinalIgnoreCase));

    private static bool IsPrefetch(HttpRequest request) =>
        request.Headers["Sec-Purpose"].ToString().Contains("prefetch", StringComparison.OrdinalIgnoreCase) ||
        request.Headers["Purpose"].ToString().Contains("prefetch", StringComparison.OrdinalIgnoreCase);
}