namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents recipe details.
/// </summary>
public sealed class RecipeDetails
{
    /// <summary>
    /// Gets or sets the prep time of a recipe detail.
    /// </summary>
    /// <value>
    /// An integer specifying the prep time of a recipe detail or possibly null.
    /// </value>
    public int? PrepTime { get; set; }

    /// <summary>
    /// Gets or sets the cook time of a recipe detail.
    /// </summary>
    /// <value>
    /// An integer specifying the cook time of a recipe detail or possibly null.
    /// </value>
    public int? CookTime { get; set; }

    /// <summary>
    /// Gets or sets the servings of a recipe detail.
    /// </summary>
    /// <value>
    /// An integer specifying the servings of a recipe detail or possibly null.
    /// </value>
    public int? Servings { get; set; }
}