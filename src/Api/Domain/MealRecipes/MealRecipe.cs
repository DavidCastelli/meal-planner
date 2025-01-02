namespace Api.Domain.MealRecipes;

/// <summary>
/// Explicit join type used to define the many-to-many relationship between a meal and a recipe.
/// </summary>
public sealed class MealRecipe
{
    /// <summary>
    /// Gets or sets the meal id of the meal recipe.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a meal.
    /// </value>
    public int MealId { get; set; }

    /// <summary>
    /// Gets or sets the recipe id of the meal recipe.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a recipe.
    /// </value>
    public int RecipeId { get; set; }
}