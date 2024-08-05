namespace Api.Domain.Recipes;

public sealed class RecipeDetails
{
    public int? PrepTime { get; set; }
    public int? CookTime { get; set; }
    public int? Servings { get; set; }
}