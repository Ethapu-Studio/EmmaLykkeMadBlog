namespace EmmaLykkeMadBlog.DTOs;

// Counts waiting to be saved for one recipe on one day.
public record RecipeViewCount(int RecipeId, DateOnly Day, int Views, int NewVisitors);