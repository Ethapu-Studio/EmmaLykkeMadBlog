using EmmaLykkeMadBlog.DTOs;

namespace EmmaLykkeMadBlog.Services.Interfaces;

// In-memory counting. Visitor values live here for the current day only and are never saved.
public interface IRecipeViewBuffer
{
    void Add(int recipeId, string visitorId, DateOnly day);

    // Takes the counts that are not saved yet and resets them. Past days are dropped afterwards.
    IReadOnlyList<RecipeViewCount> Drain(DateOnly today);

    // Puts counts back when saving failed, so they are tried again.
    void Restore(IReadOnlyList<RecipeViewCount> counts);
}