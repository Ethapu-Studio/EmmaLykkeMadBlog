namespace EmmaLykkeMadBlog.Utility.Models;

// The answer to login and refresh. ASP.NET Core's bearer token handler writes it;
// the frontend reads it with this type.
public record AuthTokenResponse(string AccessToken, string RefreshToken, long ExpiresIn);
