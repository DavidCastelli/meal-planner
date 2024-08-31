using Api.Domain.Recipes;
using Api.Domain.Tags;

namespace Api.Domain.Meals;

/// <summary>
/// Entity which represents a meal.
/// </summary>
public sealed class Meal
{
    /// <summary>
    /// Gets or sets the id of a meal.
    /// </summary>
    /// <value>
    /// An integer specifying the id of a meal.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of a meal.
    /// </summary>
    /// <value>
    /// A string specifying the title of a meal.
    /// </value>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the image of a meal.
    /// </summary>
    /// <value>
    /// A string specifying a valid url to an image of the meal or possibly null.
    /// </value>
    public string? Image { get; set; }

    /// <summary>
    /// Gets a list of tags belonging to the meal.
    /// </summary>
    /// <value>
    /// A collection of tags.
    /// </value>
    /// <see cref="Tag"/>
    public ICollection<Tag> Tags { get; } = new List<Tag>();

    /// <summary>
    /// Gets a list of recipes belonging to the meal.
    /// </summary>
    /// <value>
    /// A collection of recipes.
    /// </value>
    /// <see cref="Recipe"/>
    public ICollection<Recipe> Recipes { get; } = new List<Recipe>();

    /// <summary>
    /// Gets or sets the application user id.
    /// </summary>
    /// <value>
    /// An integer representing the id of a user who the meal belongs to.
    /// </value>
    public required int ApplicationUserId { get; set; }
}