using Api.Domain.FoodGroups;
using Api.Domain.Todos;
using Api.Domain.Identity;
using Api.Domain.Ingredients;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.Tags;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public sealed class MealPlannerContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public MealPlannerContext(DbContextOptions<MealPlannerContext> options)
        : base(options)
    {
    }

    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<SubIngredient> Ingredients => Set<SubIngredient>();
    public DbSet<FoodGroup> FoodGroups => Set<FoodGroup>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Meal>(b =>
        {
            b.Property(m => m.Title)
                .HasMaxLength(20);
            b.Property(m => m.Image)
                .HasMaxLength(255);
        });

        builder.Entity<Recipe>(b =>
        {
            b.OwnsOne(r => r.RecipeDetails);
            b.OwnsOne(r => r.RecipeNutrition);
            
            b.OwnsMany(r => r.Directions)
                .Property(d => d.Description)
                .HasMaxLength(255);
            b.OwnsMany(r => r.Tips)
                .Property(t => t.Description)
                .HasMaxLength(255);

            b.Property(r => r.Title)
                .HasMaxLength(20);
            b.Property(r => r.Description)
                .HasMaxLength(255);
            b.Property(r => r.Image)
                .HasMaxLength(255);

        });

        builder.Entity<SubIngredient>(b =>
        {
            b.OwnsMany(s => s.Ingredients, a =>
            {
                a.Property(i => i.Name)
                    .HasMaxLength(20);
                a.Property(i => i.Measurement)
                    .HasMaxLength(20);
            });

            b.Property(s => s.Name)
                .HasMaxLength(20);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .EnableSensitiveDataLogging();
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}