namespace EmmaLykkeMadBlog.DTOs;

// One recipe's place in the ranking: the sum of unique visitors over the ranking window.
public record RecipeRanking(int RecipeId, int Score);