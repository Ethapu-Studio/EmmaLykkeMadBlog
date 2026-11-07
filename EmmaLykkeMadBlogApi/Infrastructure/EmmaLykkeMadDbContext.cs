using Microsoft.EntityFrameworkCore;
using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Infrastructure;

public partial class EmmaLykkeMadDbContext : DbContext
{
    public EmmaLykkeMadDbContext(DbContextOptions<EmmaLykkeMadDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Recipe> Recipe { get; set; } = null!;
    public virtual DbSet<Category> Category { get; set; } = null!;
    public virtual DbSet<Tag> Tag { get; set; } = null!;
    public virtual DbSet<RecipeTag> RecipeTag { get; set; } = null!;
    public virtual DbSet<RecipeIngredient> RecipeIngredient { get; set; } = null!;
    public virtual DbSet<RecipeStep> RecipeStep { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}