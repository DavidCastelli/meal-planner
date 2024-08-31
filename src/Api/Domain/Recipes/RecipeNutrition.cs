namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents recipe nutrition.
/// </summary>
public sealed class RecipeNutrition
{
    /// <summary>
    /// Gets or sets calories of a recipe nutrition.
    /// </summary>
    /// <value>
    /// An integer specifying the calories of a recipe nutrition or possibly null.
    /// </value>
    public int? Calories { get; set; }

    /// <summary>
    /// Gets or sets fat of a recipe nutrition.
    /// </summary>
    /// <value>
    /// An integer specifying the fat of a recipe nutrition or possibly null.
    /// </value>
    public int? Fat { get; set; }

    /// <summary>
    /// Gets or sets the carbs of a recipe nutrition.
    /// </summary>
    /// <value>
    /// An integer specifying the carbs of a recipe nutrition or possibly null.
    /// </value>
    public int? Carbs { get; set; }

    /// <summary>
    /// Gets or sets the protein of a recipe nutrition.
    /// </summary>
    /// <value>
    /// An integer specifying the protein of a recipe nutrition or possibly null.
    /// </value>
    public int? Protein { get; set; }
}