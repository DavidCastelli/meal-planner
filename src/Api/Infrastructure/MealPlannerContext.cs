using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.Tags;
using Api.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

/// <summary>
/// Class for the Entity Framework database context used for this application.
/// </summary>
public sealed class MealPlannerContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    /// <summary>
    /// Creates a <see cref="MealPlannerContext"/>.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public MealPlannerContext(DbContextOptions<MealPlannerContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the set of <see cref="Meal"/> entities.
    /// </summary>
    /// <value>
    /// A set of meals.
    /// </value>
    public DbSet<Meal> Meals => Set<Meal>();

    /// <summary>
    /// Gets the set of <see cref="Tag"/> entities.
    /// </summary>
    /// <value>
    /// A set of tags.
    /// </value>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    /// Gets the set of <see cref="Recipe"/> entities.
    /// </summary>
    /// <value>
    /// A set of recipes.
    /// </value>
    public DbSet<Recipe> Recipes => Set<Recipe>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.HasMany<Meal>()
                .WithOne();

            b.HasMany<Recipe>()
                .WithOne();
        });

        builder.Entity<Meal>(b =>
        {
            b.Property(m => m.Title)
                .HasMaxLength(20);
            b.Property(m => m.ImagePath)
                .HasMaxLength(255);
        });

        builder.Entity<Recipe>(b =>
        {
            b.Property(r => r.Title)
                .HasMaxLength(20);
            b.Property(r => r.Description)
                .HasMaxLength(255);
            b.Property(r => r.ImagePath)
                .HasMaxLength(255);

            b.OwnsOne(r => r.RecipeDetails);
            b.OwnsOne(r => r.RecipeNutrition);

            b.OwnsMany(r => r.SubIngredients, sib =>
            {
                sib.Property(si => si.Name)
                    .HasMaxLength(20);

                sib.OwnsMany(si => si.Ingredients, ib =>
                {
                    ib.Property(i => i.Name)
                        .HasMaxLength(20);
                    ib.Property(i => i.Measurement)
                        .HasMaxLength(20);
                });
            });

            b.OwnsMany(r => r.Directions)
                .Property(d => d.Description)
                .HasMaxLength(255);

            b.OwnsMany(r => r.Tips)
                .Property(t => t.Description)
                .HasMaxLength(255);
        });
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .EnableSensitiveDataLogging();
    }
}