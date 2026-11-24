using EmmaLykkeMadBlog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace EmmaLykkeMadBlog.Infrastructure.Seeding;

// DEVELOPMENT ONLY: sample recipes so the front page has something to show.
// Runs only when the recipe table is empty. The nutrition values are made up for testing
// and are NOT real nutritional information.
public static class DevelopmentRecipeSeeder
{
    private sealed record Ing(string Name, decimal? Amount = null, string? Unit = null, string? Note = null, string? Group = null);

    private sealed record Nutrition(decimal Kcal, decimal Protein, decimal Carbs, decimal Fat,
        decimal Fiber, decimal Sugar, decimal Salt, decimal SaturatedFat);

    public static async Task<int> SeedAsync(EmmaLykkeMadDbContext db, CancellationToken ct = default)
    {
        if (await db.Recipe.AnyAsync(ct)) return 0;

        var morgenmad = await GetOrAddCategoryAsync(db, "Morgenmad", "morgenmad", ct);
        var snacks = await GetOrAddCategoryAsync(db, "Snacks", "snacks", ct);
        var aftensmad = await GetOrAddCategoryAsync(db, "Aftensmad", "aftensmad", ct);
        var bagning = await GetOrAddCategoryAsync(db, "Bagning", "bagning", ct);

        var hurtig = await GetOrAddTagAsync(db, "Hurtig", "hurtig", ct);
        var vegetar = await GetOrAddTagAsync(db, "Vegetar", "vegetar", ct);
        var fiberrig = await GetOrAddTagAsync(db, "Fiberrig", "fiberrig", ct);
        var protein = await GetOrAddTagAsync(db, "Højt proteinindhold", "hojt-proteinindhold", ct);

        await db.SaveChangesAsync(ct);

        var recipes = new List<Recipe>
        {
            Build(
                "Ovnbagt laks med citron og urter", "ovnbagt-laks-med-citron-og-urter", aftensmad,
                "En enkel bageplade-ret med sprøde grøntsager og masser af friske urter.",
                "Laksen og grøntsagerne bages sammen på én plade, så der næsten ingen opvask er. Retten er mættende, og friske urter og citron giver masser af smag.",
                10, 25, 4, new Nutrition(480, 34, 22, 27, 6, 7, 0.9m, 5), 1,
                [protein, fiberrig],
                [
                    new("Laksfilet", 600, "g"),
                    new("Kartofler", 500, "g", "små, skåret i både"),
                    new("Broccoli", 300, "g", "i buketter"),
                    new("Citron", 1, "stk.", "i tynde skiver"),
                    new("Olivenolie", 3, "spsk."),
                    new("Frisk dild", 1, "bundt", "hakket"),
                    new("Frisk persille", 1, "bundt", "hakket"),
                    new("Salt", 1, "tsk."),
                    new("Peber", 0.5m, "tsk.")
                ],
                [
                    "Tænd ovnen på 200 grader.",
                    "Vend kartoflerne med halvdelen af olien, salt og peber på en bageplade, og bag dem i 10 minutter.",
                    "Tag pladen ud, tilføj broccoli, og vend det hele. Læg laksen og citronskiverne ovenpå, og dryp med resten af olien.",
                    "Bag i 12 til 15 minutter, til laksen lige akkurat skiller sig i flager.",
                    "Drys med dild og persille, og server straks."
                ]),

            Build(
                "Havregrød med bagte æbler", "havregroed-med-bagte-aebler", morgenmad,
                "Varm og mættende morgenmad med kanel og knasende nødder.",
                "En klassiker med et twist: æblerne bages i ovnen, mens grøden koger, så de bliver bløde og søde af sig selv.",
                5, 12, 2, new Nutrition(360, 12, 48, 13, 8, 17, 0.3m, 2.5m), 3,
                [fiberrig, vegetar],
                [
                    new("Havregryn", 80, "g"),
                    new("Mælk eller havredrik", 4, "dl"),
                    new("Æbler", 2, "stk.", "i både"),
                    new("Kanel", 1, "tsk."),
                    new("Hakkede valnødder", 30, "g"),
                    new("Salt", 1, "knivspids")
                ],
                [
                    "Tænd ovnen på 200 grader. Vend æblebådene med kanel, og bag dem i 12 minutter.",
                    "Kog havregryn, mælk og salt i 5 til 8 minutter, mens du rører jævnligt.",
                    "Server grøden med de bagte æbler og hakkede nødder."
                ]),

            Build(
                "Grove rugboller", "grove-rugboller", bagning,
                "Saftige boller med frø, der holder sig friske i flere dage.",
                "Boller med meget fiber og frø i dejen. Tiden på 90 minutter er inklusive hævning.",
                70, 20, 12, new Nutrition(175, 6, 28, 4, 4.5m, 1, 0.5m, 0.5m), 6,
                [fiberrig, vegetar],
                [
                    new("Hvedemel", 300, "g", null, "Dej"),
                    new("Groft rugmel", 200, "g", null, "Dej"),
                    new("Tørgær", 1, "pose", "à 12 g", "Dej"),
                    new("Lunkent vand", 4, "dl", null, "Dej"),
                    new("Salt", 2, "tsk.", null, "Dej"),
                    new("Honning", 1, "spsk.", null, "Dej"),
                    new("Solsikkekerner", 50, "g", null, "Dej"),
                    new("Hørfrø", 30, "g", null, "Dej"),
                    new("Havregryn", 2, "spsk.", null, "Til toppen"),
                    new("Vand", null, null, "til pensling", "Til toppen")
                ],
                [
                    "Bland mel, gær, salt, solsikkekerner og hørfrø. Tilsæt vand og honning, og ælt dejen i 8 til 10 minutter.",
                    "Lad dejen hæve tildækket i 40 minutter, til den er dobbelt så stor.",
                    "Tænd ovnen på 225 grader. Del dejen i 12 stykker, og form dem til boller på bagepapir.",
                    "Pensl bollerne med vand, drys med havregryn, og lad dem hæve i 15 minutter.",
                    "Bag i 18 til 20 minutter, til de er gyldne. Afkøl på en rist."
                ]),

            Build(
                "Overnight oats med bær", "overnight-oats-med-baer", morgenmad,
                "Forbered aftenen før, og start dagen med ro.",
                "Du laver det aftenen før, og så er morgenmaden klar, når du vågner.",
                10, 0, 2, new Nutrition(330, 17, 42, 10, 9, 13, 0.2m, 3), 9,
                [hurtig, vegetar, fiberrig],
                [
                    new("Havregryn", 100, "g"),
                    new("Yoghurt naturel", 2.5m, "dl"),
                    new("Mælk", 1, "dl"),
                    new("Chiafrø", 1, "spsk."),
                    new("Frosne bær", 150, "g", "optøede"),
                    new("Vaniljeekstrakt", 0.5m, "tsk.")
                ],
                [
                    "Rør havregryn, yoghurt, mælk, chiafrø og vanilje sammen.",
                    "Fordel blandingen i to glas, og læg bærrene ovenpå.",
                    "Sæt glassene i køleskabet natten over, og rør rundt inden servering."
                ]),

            Build(
                "Hummus med grøntsagsstave", "hummus-med-groentsagsstave", snacks,
                "Cremet snack, der holder sulten væk.",
                "Hjemmelavet hummus er hurtig at lave og meget mere cremet end den fra køledisken.",
                10, 0, 4, new Nutrition(230, 9, 24, 11, 8, 6, 0.6m, 1.5m), 12,
                [hurtig, vegetar, fiberrig],
                [
                    new("Kikærter", 400, "g", "fra dåse, skyllet og afdryppet", "Hummus"),
                    new("Tahin", 3, "spsk.", null, "Hummus"),
                    new("Citronsaft", 3, "spsk.", null, "Hummus"),
                    new("Hvidløg", 1, "fed", null, "Hummus"),
                    new("Olivenolie", 2, "spsk.", null, "Hummus"),
                    new("Koldt vand", 3, "spsk.", "efter behov", "Hummus"),
                    new("Spidskommen", 0.5m, "tsk.", null, "Hummus"),
                    new("Salt", 0.5m, "tsk.", null, "Hummus"),
                    new("Gulerødder", 3, "stk.", "i stave", "Til servering"),
                    new("Agurk", 0.5m, "stk.", "i stave", "Til servering"),
                    new("Rød peberfrugt", 1, "stk.", "i strimler", "Til servering")
                ],
                [
                    "Blend kikærter, tahin, citronsaft, hvidløg, olie, spidskommen og salt, til det er glat.",
                    "Tilsæt koldt vand lidt ad gangen, til hummusen har den konsistens, du kan lide.",
                    "Server med grøntsagsstave."
                ]),

            Build(
                "Linsegryde med citron", "linsegryde-med-citron", aftensmad,
                "Varm gryderet fyldt med fibre og protein.",
                "En billig og mættende gryderet uden kød, hvor citronen holder smagen frisk.",
                10, 30, 4, new Nutrition(340, 19, 52, 5, 14, 9, 1.1m, 0.8m), 15,
                [vegetar, fiberrig],
                [
                    new("Røde linser", 250, "g"),
                    new("Løg", 1, "stk.", "hakket"),
                    new("Gulerødder", 2, "stk.", "i tern"),
                    new("Hvidløg", 2, "fed", "presset"),
                    new("Hakkede tomater", 400, "g", "fra dåse"),
                    new("Grøntsagsbouillon", 7, "dl"),
                    new("Spidskommen", 1, "tsk."),
                    new("Frisk spinat", 100, "g"),
                    new("Citron", 1, "stk.", "saft og revet skal"),
                    new("Olivenolie", 1, "spsk.")
                ],
                [
                    "Svits løg og gulerødder i olien i 5 minutter.",
                    "Tilsæt hvidløg og spidskommen, og rør rundt i 1 minut.",
                    "Tilsæt linser, tomater og bouillon, og lad det koge ved svag varme i 20 minutter.",
                    "Rør spinat, citronsaft og citronskal i, og smag til med salt og peber."
                ])
        };

        db.Recipe.AddRange(recipes);
        await db.SaveChangesAsync(ct);
        return recipes.Count;
    }

    private static Recipe Build(
        string title, string slug, Category category, string shortDescription, string introduction,
        int prepMinutes, int cookMinutes, int servings, Nutrition n, int publishedDaysAgo,
        Tag[] tags, Ing[] ingredients, string[] steps)
    {
        var published = DateTime.UtcNow.AddDays(-publishedDaysAgo);

        return new Recipe
        {
            Title = title,
            Slug = slug,
            Category = category,
            ShortDescription = shortDescription,
            Introduction = introduction,
            PrepTimeMinutes = prepMinutes,
            CookTimeMinutes = cookMinutes,
            Servings = servings,
            Calories = n.Kcal,
            ProteinG = n.Protein,
            CarbsG = n.Carbs,
            FatG = n.Fat,
            FiberG = n.Fiber,
            SugarG = n.Sugar,
            SaltG = n.Salt,
            SaturatedFatG = n.SaturatedFat,
            IsPublished = true,
            PublishedAt = published,
            CreatedAt = published,
            UpdatedAt = published,
            Ingredients = ingredients
                .Select((i, index) => new RecipeIngredient
                {
                    Name = i.Name,
                    Amount = i.Amount,
                    Unit = i.Unit,
                    Note = i.Note,
                    GroupName = i.Group,
                    SortOrder = index + 1
                })
                .ToList(),
            Steps = steps
                .Select((text, index) => new RecipeStep { StepNumber = index + 1, Instruction = text })
                .ToList(),
            RecipeTags = tags.Select(t => new RecipeTag { Tag = t }).ToList()
        };
    }

    private static async Task<Category> GetOrAddCategoryAsync(
        EmmaLykkeMadDbContext db, string name, string slug, CancellationToken ct)
    {
        var existing = await db.Category.FirstOrDefaultAsync(c => c.Slug == slug || c.Name == name, ct);
        if (existing is not null) return existing;

        var created = new Category { Name = name, Slug = slug };
        db.Category.Add(created);
        return created;
    }

    private static async Task<Tag> GetOrAddTagAsync(
        EmmaLykkeMadDbContext db, string name, string slug, CancellationToken ct)
    {
        var existing = await db.Tag.FirstOrDefaultAsync(t => t.Slug == slug || t.Name == name, ct);
        if (existing is not null) return existing;

        var created = new Tag { Name = name, Slug = slug };
        db.Tag.Add(created);
        return created;
    }
}