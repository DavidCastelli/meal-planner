namespace Api.Domain.Recipes;

public sealed class RecipeNutrition
{
    public int? Calories { get; set; }
    public int? Fat { get; set; }
    public int? Carbs { get; set; }
    public int? Protein { get; set; }
}