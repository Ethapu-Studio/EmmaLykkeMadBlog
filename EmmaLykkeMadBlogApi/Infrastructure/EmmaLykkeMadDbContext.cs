using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmmaLykkeMadBlog.Infrastructure.Models;

namespace EmmaLykkeMadBlog.Infrastructure;

public partial class EmmaLykkeMadDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public EmmaLykkeMadDbContext(DbContextOptions<EmmaLykkeMadDbContext> options) : base(options)
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

        // Identity tables get snake_case names. Only these entities are touched,
        // so the existing recipe tables are not affected.
        modelBuilder.Entity<AppUser>().ToTable("app_user");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("app_role");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("app_user_role");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("app_user_claim");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("app_user_login");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("app_user_token");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("app_role_claim");

        var identityTypes = new[]
        {
            typeof(AppUser), typeof(IdentityRole<Guid>), typeof(IdentityUserRole<Guid>),
            typeof(IdentityUserClaim<Guid>), typeof(IdentityUserLogin<Guid>),
            typeof(IdentityUserToken<Guid>), typeof(IdentityRoleClaim<Guid>)
        };

        foreach (var entity in modelBuilder.Model.GetEntityTypes()
                     .Where(e => identityTypes.Contains(e.ClrType)))
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(
                    Regex.Replace(property.Name, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant());
            }
        }
    }
}