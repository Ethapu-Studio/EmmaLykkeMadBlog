using EmmaLykkeMadBlog.Services.Interfaces;
using EmmaLykkeMadBlog.Utility.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmmaLykkeMadBlog.Controllers;

// Public and read-only. Only published recipes are ever returned.
[ApiController]
[AllowAnonymous]
[Route("api/recipes")]
public class RecipesController(IRecipeService recipes, IRecipeViewService views) : ControllerBase
{
    [HttpGet("home")]
    public async Task<ActionResult<RecipeHomeResponse>> Home(CancellationToken ct) =>
        Ok(await recipes.GetHomeAsync(ct));
    
    // Public search, filter, sort and paging. Input is validated by the request model.
    [HttpGet]
    public async Task<ActionResult<RecipeListResponse>> List([FromQuery] RecipeListRequest request, CancellationToken ct) =>
        Ok(await recipes.GetListAsync(request, ct));

    // Missing and unpublished recipes give the same 404.
    [HttpGet("{slug}")]
    public async Task<ActionResult<RecipeDetailResponse>> Detail(string slug, CancellationToken ct)
    {
        var recipe = await recipes.GetDetailAsync(slug, ct);
        return recipe is null ? NotFound() : Ok(recipe);
    }
    
    // Called by the frontend server when a recipe page renders. Input is validated by the request model.
    [HttpPost("{slug}/view")]
    public async Task<IActionResult> View(string slug, [FromBody] RecipeViewRequest request, CancellationToken ct) =>
        await views.RegisterViewAsync(slug, request.VisitorId, ct) ? NoContent() : NotFound();
}
