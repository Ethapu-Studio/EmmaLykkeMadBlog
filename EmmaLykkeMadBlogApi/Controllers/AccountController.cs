using System.Security.Claims;
using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmmaLykkeMadBlog.Controllers;

[ApiController]
[Authorize]
[Route("api/account")]
public class AccountController(IAccountService accounts) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<AccountMeResponse>> Me(CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var me = await accounts.GetMeAsync(userId, ct);
        return me is null ? Unauthorized() : Ok(me);
    }
}
