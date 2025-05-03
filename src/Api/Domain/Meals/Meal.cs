using Api.Domain.ManageableEntities;
using Api.Domain.Recipes;
using Api.Domain.Tags;

namespace Api.Domain.Meals;

/// <summary>
/// Entity which represents a meal.
/// </summary>
public sealed class Meal : ManageableEntity
{

    /// <summary>
    /// Creates a <see cref="Meal"/>.
    /// </summary>
    public Meal() { }

    /// <summary>
    /// Creates a <see cref="Meal"/> given a collection of <paramref name="tags"/> and a collection of <paramref name="recipes"/>.
    /// </summary>
    /// <param name="tags">The meal tags.</param>
    /// <param name="recipes">The meal recipes.</param>
    public Meal(ICollection<Tag> tags, ICollection<Recipe> recipes)
    {
        Tags = tags;
        Recipes = recipes;
    }
    
    /// <summary>
    /// Gets or sets when the meal is scheduled for the week.
    /// </summary>
    /// <value>
    /// A schedule enum.
    /// </value>
    /// <see cref="Meals.Schedule"/>
    public Schedule Schedule { get; set; }

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
}