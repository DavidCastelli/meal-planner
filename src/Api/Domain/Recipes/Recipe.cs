using Api.Domain.Identity;
using Api.Domain.Ingredients;
using Api.Domain.Meals;

namespace Api.Domain.Recipes;

public sealed class Recipe
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public required RecipeDetails RecipeDetails { get; set; }
    public required RecipeNutrition RecipeNutrition { get; set; }
    public IList<Direction> Directions { get; } = new List<Direction>();
    public ICollection<Tip> Tips { get; } = new List<Tip>();
    public ICollection<Meal> Meals { get; } = new List<Meal>();
    public ICollection<SubIngredient> SubIngredients { get; } = new List<SubIngredient>();
    public int ApplicationUserId { get; set; }
    public required ApplicationUser User { get; set; }
}