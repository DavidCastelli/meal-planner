using Api.Domain.Meals;

namespace Api.Domain.Tags;

/// <summary>
/// Entity which represents a tag.
/// </summary>
/// <remarks>
/// Used to help filter items.
/// </remarks>
public sealed class Tag
{
    /// <summary>
    /// Gets or sets the id of the tag.
    /// </summary>
    /// <value>
    /// An integer specifying the id of the tag.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="TagType"/>.
    /// </summary>
    /// <value>
    /// An enum specifying the type of tag.
    /// </value>
    public TagType Type { get; set; }

    /// <summary>
    /// Gets a list of meals which contain the tag.
    /// </summary>
    /// <value>
    /// A collection of meals.
    /// </value>
    /// <see cref="Meal"/>
    public ICollection<Meal> Meals { get; } = new List<Meal>();
}