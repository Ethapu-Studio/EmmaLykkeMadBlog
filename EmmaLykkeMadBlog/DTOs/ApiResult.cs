namespace EmmaLykkeMadBlog.DTOs;

// The outcome of a call to the API: success, or the error code the API returned.
public record ApiResult(bool Ok, string? Code = null);
