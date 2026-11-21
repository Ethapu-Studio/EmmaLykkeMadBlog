using EmmaLykkeMadBlog.Contracts;
using EmmaLykkeMadBlog.Repositories;

namespace EmmaLykkeMadBlog.Services;

public interface IAccountService
{
    Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct);
}

public class AccountService(IAccountRepository accounts) : IAccountService
{
    public Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct) =>
        accounts.GetProfileAsync(userId, ct);
}