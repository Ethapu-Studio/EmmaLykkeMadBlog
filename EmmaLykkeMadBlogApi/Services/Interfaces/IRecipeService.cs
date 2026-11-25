using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IRecipeService
{
    Task<RecipeHomeResponse> GetHomeAsync(CancellationToken ct);
}
