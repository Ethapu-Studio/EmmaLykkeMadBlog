namespace EmmaLykkeMadBlog.DTOs;

public record EmailMessage(
    string ToAddress,
    string? ToName,
    string Subject,
    string HtmlBody,
    string? PlainTextBody = null);
