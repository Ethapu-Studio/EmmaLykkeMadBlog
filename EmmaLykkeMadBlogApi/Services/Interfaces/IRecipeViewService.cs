namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IRecipeViewService
{
    // True when the recipe exists and is published (the view is then counted).
    Task<bool> RegisterViewAsync(string slug, string visitorId, CancellationToken ct);
}