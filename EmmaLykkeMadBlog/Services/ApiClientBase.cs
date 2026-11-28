using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmmaLykkeMadBlog.DTOs;

namespace EmmaLykkeMadBlog.Services;

// Shared handling for every API client: status mapping, error handling and logging in one place.
// Expected failures (timeout, network, 5xx, bad JSON) become Unavailable and never throw into a page.
// A cancelled request (the visitor left) is not an error and is passed on.
// Logs hold the operation name and a status code or error type only: never bodies, headers or tokens.
public abstract class ApiClientBase(HttpClient http, ILogger logger)
{
    protected async Task<ApiResponse<T>> GetAsync<T>(string operation, string relativeUrl, CancellationToken ct)
        where T : class
    {
        try
        {
            using var response = await http.GetAsync(relativeUrl, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new ApiResponse<T>(ApiStatus.NotFound);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("API call {Operation} returned status {Status}", operation, (int)response.StatusCode);
                return new ApiResponse<T>(ApiStatus.Unavailable);
            }

            var value = await response.Content.ReadFromJsonAsync<T>(ct);
            if (value is null)
            {
                logger.LogWarning("API call {Operation} returned an empty body", operation);
                return new ApiResponse<T>(ApiStatus.Unavailable);
            }

            return new ApiResponse<T>(ApiStatus.Ok, value);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or JsonException or NotSupportedException ||
            (ex is OperationCanceledException && !ct.IsCancellationRequested)) // timeout, not a leaving visitor
        {
            logger.LogWarning("API call {Operation} failed: {Error}", operation, ex.GetType().Name);
            return new ApiResponse<T>(ApiStatus.Unavailable);
        }
    }

    
    // For calls that only need an outcome: Ok (any 2xx), NotFound or Unavailable. Same error handling as GetAsync.
    protected async Task<ApiStatus> PostAsync<TBody>(
        string operation, string relativeUrl, TBody body, CancellationToken ct)
    {
        try
        {
            using var response = await http.PostAsJsonAsync(relativeUrl, body, ct);

            if (response.IsSuccessStatusCode) return ApiStatus.Ok;
            if (response.StatusCode == HttpStatusCode.NotFound) return ApiStatus.NotFound;

            logger.LogWarning("API call {Operation} returned status {Status}", operation, (int)response.StatusCode);
            return ApiStatus.Unavailable;
        }
        catch (Exception ex) when (
            ex is HttpRequestException ||
            (ex is OperationCanceledException && !ct.IsCancellationRequested))
        {
            logger.LogWarning("API call {Operation} failed: {Error}", operation, ex.GetType().Name);
            return ApiStatus.Unavailable;
        }
    }
}