using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services;

// Public, read-only recipe data. Talks to the API from the frontend's SERVER; the browser never calls the API.
public class RecipeApiClient(HttpClient http, ILogger<RecipeApiClient> logger)
    : ApiClientBase(http, logger), IRecipeApiClient
{
    public Task<ApiResponse<RecipeHomeResponse>> GetHomeAsync(CancellationToken ct) =>
        GetAsync<RecipeHomeResponse>("recipes.home", "api/recipes/home", ct);

    public Task<ApiResponse<RecipeDetailResponse>> GetRecipeAsync(string slug, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Task.FromResult(new ApiResponse<RecipeDetailResponse>(ApiStatus.NotFound));

        // The slug comes from the address bar, so it is escaped into a relative URL, never concatenated raw.
        return GetAsync<RecipeDetailResponse>(
            "recipes.detail", $"api/recipes/{Uri.EscapeDataString(slug)}", ct);
    }

    
    public Task<ApiStatus> TrackViewAsync(string slug, string visitorId, CancellationToken ct) =>
        PostAsync("recipes.view",
            $"api/recipes/{Uri.EscapeDataString(slug)}/view",
            new RecipeViewRequest { VisitorId = visitorId },
            ct);
}