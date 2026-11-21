namespace EmmaLykkeMadBlog.Utility.Models;

public record AuthErrorResponse(string Code, string[]? Errors = null);
