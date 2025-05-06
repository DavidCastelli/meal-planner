using Api.Domain.Images;
using Api.Domain.ManageableEntities;
using Api.Domain.MealRecipes;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.ShoppingItems;
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
    /// Gets the set of <see cref="Domain.ManageableEntities.ManageableEntity"/> entities.
    /// </summary>
    /// <value>
    /// A set of manageable entities.
    /// </value>
    public DbSet<ManageableEntity> ManageableEntity => Set<ManageableEntity>();

    /// <summary>
    /// Gets the set of <see cref="Domain.Meals.Meal"/> entities.
    /// </summary>
    /// <value>
    /// A set of meals.
    /// </value>
    public DbSet<Meal> Meal => Set<Meal>();

    /// <summary>
    /// Gets the set of <see cref="Domain.Tags.Tag"/> entities.
    /// </summary>
    /// <value>
    /// A set of tags.
    /// </value>
    public DbSet<Tag> Tag => Set<Tag>();

    /// <summary>
    /// Gets the set of <see cref="Domain.Recipes.Recipe"/> entities.
    /// </summary>
    /// <value>
    /// A set of recipes.
    /// </value>
    public DbSet<Recipe> Recipe => Set<Recipe>();

    /// <summary>
    /// Gets the set of <see cref="Domain.MealRecipes.MealRecipe"/> entities.
    /// </summary>
    /// <value>
    /// A set of meal recipe.
    /// </value>
    public DbSet<MealRecipe> MealRecipe => Set<MealRecipe>();

    /// <summary>
    /// Gets the set of <see cref="Domain.Images.Image"/> entities.
    /// </summary>
    /// <value>
    /// A set of images.
    /// </value>
    public DbSet<Image> Image => Set<Image>();

    /// <summary>
    /// Gets the set of <see cref="Domain.ShoppingItems.ShoppingItem"/> entities.
    /// </summary>
    /// <value>
    /// A set of shopping items.
    /// </value>
    public DbSet<ShoppingItem> ShoppingItem => Set<ShoppingItem>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasCollation("case_insensitive", "en-u-ks-primary", "icu", false);
        builder.HasPostgresEnum<TagType>();
        builder.HasPostgresEnum<Schedule>();

        builder.Entity<ApplicationUser>(b =>
        {
            b.HasMany<ManageableEntity>()
                .WithOne();

            b.HasMany<ShoppingItem>()
                .WithOne();
        });

        builder.Entity<ManageableEntity>(b =>
        {
            b.UseTptMappingStrategy();

            b.HasIndex(r => new { r.ApplicationUserId, r.Title })
                .IsUnique();

            b.Property(me => me.Title)
                .HasMaxLength(20)
                .UseCollation("case_insensitive");
        });

        builder.Entity<Meal>(b =>
        {
            b.HasBaseType<ManageableEntity>();

            b.HasMany(m => m.Recipes)
                .WithMany(r => r.Meals)
                .UsingEntity<MealRecipe>();
        });

        builder.Entity<Recipe>(b =>
        {
            b.HasBaseType<ManageableEntity>();

            b.Property(r => r.Description)
                .HasMaxLength(255);

            b.OwnsOne(r => r.RecipeDetails);
            b.OwnsOne(r => r.RecipeNutrition);

            b.OwnsMany(r => r.Directions, db =>
            {
                db.HasIndex("RecipeId", "Number")
                    .IsUnique();

                db.Property(d => d.Description)
                    .HasMaxLength(255);
            });

            b.OwnsMany(r => r.Tips)
                .Property(t => t.Description)
                .HasMaxLength(255);

            b.OwnsMany(r => r.SubIngredients, sib =>
            {
                sib.HasIndex("RecipeId", "Name")
                    .IsUnique();

                sib.Property(si => si.Name)
                    .HasMaxLength(20)
                    .UseCollation("case_insensitive");

                sib.OwnsMany(si => si.Ingredients, ib =>
                {
                    ib.Property(i => i.Name)
                        .HasMaxLength(20);
                    ib.Property(i => i.Measurement)
                        .HasMaxLength(20);
                });
            });
        });

        builder.Entity<Tag>(b =>
        {
            b.HasData(
                new Tag { Id = 1, Type = TagType.Carnivore },
                new Tag { Id = 2, Type = TagType.Vegetarian },
                new Tag { Id = 3, Type = TagType.Vegan },
                new Tag { Id = 4, Type = TagType.Breakfast },
                new Tag { Id = 5, Type = TagType.Lunch },
                new Tag { Id = 6, Type = TagType.Dinner },
                new Tag { Id = 7, Type = TagType.Supper });
        });

        builder.Entity<Image>(b =>
        {
            b.HasIndex(i => i.StorageFileName)
                .IsUnique();

            b.Property(i => i.StorageFileName)
                .HasMaxLength(12);
            b.Property(i => i.DisplayFileName)
                .HasMaxLength(255);
            b.Property(i => i.ImagePath)
                .HasMaxLength(255);
            b.Property(i => i.ImageUrl)
                .HasMaxLength(255);
        });

        builder.Entity<ShoppingItem>(b =>
        {
            b.Property(si => si.Name)
                .HasMaxLength(20);

            b.Property(si => si.Measurement)
                .HasMaxLength(20);

            b.Property(si => si.Price)
                .HasPrecision(12, 2);
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