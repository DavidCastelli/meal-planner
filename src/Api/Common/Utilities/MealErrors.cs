namespace Api.Common.Utilities;

/// <summary>
/// Utility class used to retrieve <see cref="Error"/> messages in relation to a meal.
/// </summary>
public static class MealErrors
{
    /// <summary>
    /// Constant defining the maximum number of meals a user can create.
    /// </summary>
    public const int MaxMealsCount = 50;

    /// <summary>
    /// Constant defining the maximum number of tags a meal can contain.
    /// </summary>
    public const int MaxTagsCount = 3;

    /// <summary>
    /// Constant defining the maximum number of recipes a meal can contain.
    /// </summary>
    public const int MaxRecipesCount = 3;

    /// <summary>
    /// Error message that is used when a user attempts to create a meal and exceeds the maximum number permitted <see cref="MaxMealsCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxMeals() => new Error(
        "Meals.MaxMeals", $"Cannot create more than the maximum number of meals: {MaxMealsCount}.");

    /// <summary>
    /// Error message that is used when a meal contains more than the maximum number of allowed tags <see cref="MaxTagsCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxTags() => new Error(
        "Meals.MaxTags", $"Meal cannot have more than {MaxTagsCount} tags.");

    /// <summary>
    /// Error message that is used when a meal contains more than the maximum number of allowed recipes <see cref="MaxRecipesCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxRecipes() => new Error(
        "Meals.MaxRecipes", $"Meal cannot have more than {MaxRecipesCount} recipes.");
}