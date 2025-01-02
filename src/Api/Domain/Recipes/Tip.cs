namespace Api.Domain.Recipes;

/// <summary>
/// Entity which represents a tip.
/// </summary>
public sealed class Tip
{
    /// <summary>
    /// Gets or sets the id of a tip.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a tip.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the description of the tip.
    /// </summary>
    /// <value>
    /// A string describing the tip.
    /// </value>
    public required string Description { get; set; }
}