namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a user attempts to delete the last recipe of a meal.
/// </summary>
public sealed class LastMealRecipeException : ConflictException
{
    /// <summary>
    /// Creates a <see cref="LastMealRecipeException"/>.
    /// </summary>
    public LastMealRecipeException()
        : base("The recipe could not be deleted as it is the last recipe of one or more meals." +
               " Either a new recipe should be added or the meal must first be deleted.")
    {
    }
}