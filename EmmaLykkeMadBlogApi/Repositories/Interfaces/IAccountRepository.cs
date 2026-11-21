using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<AccountMeResponse?> GetProfileAsync(Guid userId, CancellationToken ct);
    Task<int> DeleteUnconfirmedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct);
}
