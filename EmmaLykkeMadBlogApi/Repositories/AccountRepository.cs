using EmmaLykkeMadBlog.Infrastructure;
using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Repositories;

// Users themselves are handled by Identity's UserManager (see AuthService).
// This repository only holds our own queries.
public class AccountRepository(EmmaLykkeMadDbContext db) : IAccountRepository
{
    public Task<AccountMeResponse?> GetProfileAsync(Guid userId, CancellationToken ct) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new AccountMeResponse(u.Id, u.DisplayName))
            .FirstOrDefaultAsync(ct);

    public Task<int> DeleteUnconfirmedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct) =>
        db.Users
            .Where(u => !u.EmailConfirmed && u.CreatedAt < cutoff)
            .ExecuteDeleteAsync(ct);
}
