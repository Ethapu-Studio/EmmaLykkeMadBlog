namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IRecipeViewTracker
{
    // Counts one page view of a recipe. Expected failures never reach the caller, and the call never waits long.
    Task TrackAsync(HttpContext http, string slug, CancellationToken ct);
}