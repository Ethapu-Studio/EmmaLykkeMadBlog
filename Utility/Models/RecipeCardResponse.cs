namespace EmmaLykkeMadBlog.Utility.Models;

// One recipe as shown on a card (front page and lists).
public record RecipeCardResponse(
    string Slug,
    string Title,
    string ShortDescription,
    string CategoryName,
    string CategorySlug,
    int TotalMinutes,
    string? ImagePath,
    string? ImageAlt);
