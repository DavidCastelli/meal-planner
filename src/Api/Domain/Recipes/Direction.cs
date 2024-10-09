namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents a direction of a recipe.
/// </summary>
public sealed class Direction
{
    /// <summary>
    /// Gets or sets the number of the direction.
    /// </summary>
    /// <remarks>
    /// Direction numbers belonging to a recipe should be sequential starting from 1.
    /// </remarks>
    /// <value>
    /// An integer indicating the order the direction should be done.
    /// </value>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the description of a direction.
    /// </summary>
    /// <value>
    /// A string describing what should be done as part of the direction.
    /// </value>
    public required string Description { get; set; }
}