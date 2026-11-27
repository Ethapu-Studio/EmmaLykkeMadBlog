namespace EmmaLykkeMadBlog.Utility.Models;

// GroupName is null for ingredients without a heading.
public record RecipeIngredientGroupResponse(
    string? GroupName,
    IReadOnlyList<RecipeIngredientResponse> Ingredients);