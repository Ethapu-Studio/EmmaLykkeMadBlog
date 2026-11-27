namespace EmmaLykkeMadBlog.DTOs;

// The outcome of a read from the API. Value is only set when Status is Ok.
public record ApiResponse<T>(ApiStatus Status, T? Value = null) where T : class;