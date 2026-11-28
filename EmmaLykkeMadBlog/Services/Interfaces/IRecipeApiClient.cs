using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IRecipeApiClient
{
    Task<ApiResponse<RecipeHomeResponse>> GetHomeAsync(CancellationToken ct);
    Task<ApiResponse<RecipeDetailResponse>> GetRecipeAsync(string slug, CancellationToken ct);
        Task<ApiStatus> TrackViewAsync(string slug, string visitorId, CancellationToken ct);
}