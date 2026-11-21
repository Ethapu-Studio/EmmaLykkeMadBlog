namespace EmmaLykkeMadBlog.DTOs;

public record ServiceResult(bool Succeeded, string? Code = null, string[]? Errors = null)
{
    public static ServiceResult Ok() => new(true);
    public static ServiceResult Fail(string code, params string[] errors) => new(false, code, errors);
}
