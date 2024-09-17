namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a meal could not be found when searched in the data store.
/// </summary>
public sealed class MealNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="MealNotFoundException"/> given a <paramref name="mealId"/>.
    /// </summary>
    /// <param name="mealId">The id of the meal that could not be found.</param>
    public MealNotFoundException(int mealId)
        : base($"Meal with id: {mealId} was not found")
    {
    }
}