using Api.Domain.ManageableEntities;
using Api.Domain.Meals;

namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents a recipe.
/// </summary>
public sealed class Recipe : ManageableEntity
{
    /// <summary>
    /// Gets or sets the description of a recipe.
    /// </summary>
    /// <value>
    /// A string describing the recipe or possibly null.
    /// </value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Api.Domain.Recipes.RecipeDetails"/> of the recipe.
    /// </summary>
    /// <value>
    /// An object which includes additional details about the recipe.
    /// </value>
    public required RecipeDetails RecipeDetails { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Api.Domain.Recipes.RecipeNutrition"/> of the recipe.
    /// </summary>
    /// <value>
    /// An object which includes nutritional information about the recipe.
    /// </value>
    public required RecipeNutrition RecipeNutrition { get; set; }

    /// <summary>
    /// Gets a list of directions belonging to the recipe.
    /// </summary>
    /// <value>
    /// A list of directions.
    /// </value>
    /// <see cref="Direction"/>
    public IList<Direction> Directions { get; } = new List<Direction>();

    /// <summary>
    /// Gets a list of tips belonging to the recipe.
    /// </summary>
    /// <value>
    /// A collection of tips.
    /// </value>
    /// <see cref="Tip"/>
    public ICollection<Tip> Tips { get; } = new List<Tip>();

    /// <summary>
    /// Gets a list of meals belonging to the recipe.
    /// </summary>
    /// <value>
    /// A collection of meals.
    /// </value>
    /// <see cref="Meal"/>
    public ICollection<Meal> Meals { get; } = new List<Meal>();

    /// <summary>
    /// Gets a list of sub ingredients belonging to the recipe.
    /// </summary>
    /// <value>
    /// A collection of sub ingredients.
    /// </value>
    /// <see cref="SubIngredient"/>
    public ICollection<SubIngredient> SubIngredients { get; } = new List<SubIngredient>();
}