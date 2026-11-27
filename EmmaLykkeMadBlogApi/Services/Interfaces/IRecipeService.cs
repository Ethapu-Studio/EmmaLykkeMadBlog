using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IRecipeService
{
    Task<RecipeHomeResponse> GetHomeAsync(CancellationToken ct);
    Task<RecipeDetailResponse?> GetDetailAsync(string slug, CancellationToken ct);
}
