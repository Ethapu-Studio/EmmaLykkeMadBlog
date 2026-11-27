namespace EmmaLykkeMadBlog.Utility.Models;

public record RecipeIngredientResponse(
    string Name, 
    decimal? Amount, 
    string? Unit, 
    string? Note);