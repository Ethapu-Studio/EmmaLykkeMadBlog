using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

public class RecipeViewBuffer : IRecipeViewBuffer
{
    // Protects memory: past this many distinct visitors per recipe per day, new ones are no longer counted as unique.
    private const int MaxVisitorsPerRecipeDay = 50_000;

    private readonly object gate = new();
    private readonly Dictionary<(int RecipeId, DateOnly Day), Entry> entries = new();

    public void Add(int recipeId, string visitorId, DateOnly day)
    {
        lock (gate)
        {
            if (!entries.TryGetValue((recipeId, day), out var entry))
                entries[(recipeId, day)] = entry = new Entry();

            entry.PendingViews++;

            if (entry.Visitors.Count < MaxVisitorsPerRecipeDay && entry.Visitors.Add(visitorId))
                entry.PendingVisitors++;
        }
    }

    public IReadOnlyList<RecipeViewCount> Drain(DateOnly today)
    {
        lock (gate)
        {
            var counts = new List<RecipeViewCount>();

            foreach (var (key, entry) in entries)
            {
                if (entry.PendingViews == 0) continue;

                counts.Add(new RecipeViewCount(key.RecipeId, key.Day, entry.PendingViews, entry.PendingVisitors));
                entry.PendingViews = 0;
                entry.PendingVisitors = 0;
            }

            // Past days are fully drained above, so their visitor sets are not needed any more.
            foreach (var key in entries.Keys.Where(k => k.Day < today).ToList())
                entries.Remove(key);

            return counts;
        }
    }

    public void Restore(IReadOnlyList<RecipeViewCount> counts)
    {
        lock (gate)
        {
            foreach (var count in counts)
            {
                if (!entries.TryGetValue((count.RecipeId, count.Day), out var entry))
                    entries[(count.RecipeId, count.Day)] = entry = new Entry();

                entry.PendingViews += count.Views;
                entry.PendingVisitors += count.NewVisitors;
            }
        }
    }

    private sealed class Entry
    {
        public HashSet<string> Visitors { get; } = new();
        public int PendingViews { get; set; }
        public int PendingVisitors { get; set; }
    }
}