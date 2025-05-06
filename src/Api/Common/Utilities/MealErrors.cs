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
    /// Constant defining the maximum number of meals that can be scheduled to a specific day of the week.
    /// </summary>
    public const int MaxScheduleCount = 1;

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

    /// <summary>
    /// Error message that is used when more than the maximum number of allowed meals <see cref="MaxScheduleCount"/> are scheduled on the same day.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxScheduled() => new Error(
        "Meals.MaxScheduled", $"Cannot have more than {MaxScheduleCount} meals scheduled on the same day.");

    /// <summary>
    /// Error message that is used when a meal is assigned to more than one recipe with the same id.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DuplicateRecipeId() => new Error(
        "Meals.DuplicateRecipeId", $"Meal cannot be assigned the same recipe id more than once.");

    /// <summary>
    /// Error message that is used when a meal is assigned to more than one tag with the same id.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DuplicateTagId() => new Error(
        "Meals.DuplicateTagId", $"Meal cannot be assigned the same tag id more than once.");
}