using EmmaLykkeMadBlog.Contracts;
using EmmaLykkeMadBlog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Repositories;

public interface IAccountRepository
{
    Task<MeResponse?> GetProfileAsync(Guid userId, CancellationToken ct);
    Task<int> DeleteUnconfirmedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct);
}

public class AccountRepository(EmmaLykkeMadDbContext db) : IAccountRepository
{
    public Task<MeResponse?> GetProfileAsync(Guid userId, CancellationToken ct) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new MeResponse(u.Id, u.DisplayName))
            .FirstOrDefaultAsync(ct);

    public Task<int> DeleteUnconfirmedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct) =>
        db.Users
            .Where(u => !u.EmailConfirmed && u.CreatedAt < cutoff)
            .ExecuteDeleteAsync(ct);
}