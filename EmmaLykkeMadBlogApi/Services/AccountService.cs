using EmmaLykkeMadBlog.Repositories.Interfaces;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;

namespace EmmaLykkeMadBlog.Services;

public class AccountService(IAccountRepository accounts) : IAccountService
{
    public Task<AccountMeResponse?> GetMeAsync(Guid userId, CancellationToken ct) =>
        accounts.GetProfileAsync(userId, ct);
}
