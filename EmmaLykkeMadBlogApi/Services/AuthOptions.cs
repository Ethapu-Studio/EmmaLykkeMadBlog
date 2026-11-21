namespace EmmaLykkeMadBlog.Services;

public class AuthOptions
{
    public const string SectionName = "Auth";

    // Where the links in emails point (the Blazor frontend, not the API).
    public string FrontendBaseUrl { get; set; } = "https://localhost:6001";
}
