using Api.Domain.Ingredients;
using Api.Domain.Meals;

namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents a recipe.
/// </summary>
public sealed class Recipe
{
    /// <summary>
    /// Gets or sets the id of a recipe.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a recipe.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of a recipe.
    /// </summary>
    /// <value>
    /// A string specifying the title of a recipe.
    /// </value>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of a recipe.
    /// </summary>
    /// <value>
    /// A string describing the recipe or possibly null.
    /// </value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the image of the recipe.
    /// </summary>
    /// <value>
    /// A string specifying a valid url to an image of the recipe or possibly null.
    /// </value>
    public string? Image { get; set; }

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

    /// <summary>
    /// Gets or sets the application user id.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a user who the recipe belongs to.
    /// </value>
    public required int ApplicationUserId { get; set; }
}