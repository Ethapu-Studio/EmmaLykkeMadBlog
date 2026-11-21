using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IAccountService
{
    Task<AccountMeResponse?> GetMeAsync(Guid userId, CancellationToken ct);
}
