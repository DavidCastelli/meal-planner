namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents an individual ingredient.
/// </summary>
public sealed class Ingredient
{
    /// <summary>
    /// Gets or sets the id of an ingredient.
    /// </summary>
    /// <value>
    /// An integer specifying the id of an ingredient.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of an ingredient.
    /// </summary>
    /// <value>
    /// A string specifying the name of an ingredient.
    /// </value>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the measurement of an ingredient.
    /// </summary>
    /// <value>
    /// A string specifying the measurement of an ingredient.
    /// </value>
    public required string Measurement { get; set; }
}