using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmmaLykkeMadBlog.Controllers;

// Public and read-only. Only published recipes are ever returned.
[ApiController]
[AllowAnonymous]
[Route("api/recipes")]
public class RecipesController(IRecipeService recipes) : ControllerBase
{
    [HttpGet("home")]
    public async Task<ActionResult<RecipeHomeResponse>> Home(CancellationToken ct) =>
        Ok(await recipes.GetHomeAsync(ct));
}
