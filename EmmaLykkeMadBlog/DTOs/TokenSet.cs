namespace EmmaLykkeMadBlog.DTOs;

// The API tokens, kept only inside the encrypted login cookie on the server.
public record TokenSet(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
