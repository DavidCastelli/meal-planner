namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents a sub ingredient.
/// </summary>
/// <remarks>
/// A sub ingredient is used to provide a name to a list of ingredients.
/// A recipe should have only one sub ingredient with no name,
/// or multiple sub ingredients each with a name specified in the case a recipe consists
/// of multiple components.
/// </remarks>
public sealed class SubIngredient
{
    /// <summary>
    /// Gets or sets the id of a sub ingredient.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a sub ingredient.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of a sub ingredient.
    /// </summary>
    /// <value>
    /// A string specifying the name of a sub ingredient or possibly null.
    /// </value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the list of ingredients belonging to the sub ingredient.
    /// </summary>
    /// <value>
    /// A collection of individual ingredients.
    /// </value>
    /// <see cref="Ingredient"/>
    public ICollection<Ingredient> Ingredients { get; } = new List<Ingredient>();
}