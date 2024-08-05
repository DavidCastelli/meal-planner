using Api.Domain.FoodGroups;
using Api.Domain.Recipes;

namespace Api.Domain.Ingredients;

public sealed class SubIngredient
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int FoodGroupId { get; set; }
    public FoodGroup? FoodGroup { get; set; }
    public ICollection<Ingredient> Ingredients { get; } = new List<Ingredient>();
    public int RecipeId { get; set; }
    public required Recipe Recipe { get; set; }
}