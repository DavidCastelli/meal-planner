namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when an image could not be found for a meal.
/// </summary>
public sealed class MealImageNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="MealImageNotFoundException"/> given a <paramref name="mealId"/>.
    /// </summary>
    /// <param name="mealId">The id of the meal whose image could not be found.</param>
    public MealImageNotFoundException(int mealId)
        : base($"No image found for meal with id: {mealId}.")
    {
    }
}