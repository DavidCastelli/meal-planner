using Api.Domain.Ingredients;

namespace Api.Domain.Recipes;

/// <summary>
/// Utility class used to retrieve <see cref="Error"/> messages in relation to a recipe.
/// </summary>
public static class RecipeErrors
{
    /// <summary>
    /// Error message that is used when a recipe's application user id does not match that of the current requests
    /// user id. 
    /// </summary>
    /// <remarks>
    /// This error message is mainly used when a user tries to create a meal and passes a recipe id which does not belong
    /// to them.
    /// </remarks>
    /// <param name="userId">The id of the user of the current request.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DoesNotBelongToUser(int userId) => new(
        "Recipes.DoesNotBelongToUser", $"One or more recipes do not belong to user: {userId}");

    /// <summary>
    /// Error message that is used when the first direction does not start at one.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DirectionsStartAtOne() => new(
        "Recipes.DirectionsStartAtOne", "Recipe directions must start at one.");

    /// <summary>
    /// Error message that is used when the recipe directions are not sequential.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DirectionsNotSequential() => new(
        "Recipes.DirectionsNotSequential", "Recipe directions must be sequential.");

    /// <summary>
    /// Error message that is used when a recipe contains multiple sub ingredients but at least one of which is not
    /// given a name.
    /// </summary>
    /// <remarks>
    /// For more information on when to use the error <see cref="SubIngredient"/>
    /// </remarks>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MultipleSubIngredientName() => new(
        "Recipes.MultipleSubIngredientName", "All sub ingredients must have a name when more than one is provided");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed directions (6).
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxDirections() => new(
    "Recipes.MaxDirections", "Recipe cannot have more than six directions.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed tips (3).
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxTips() => new(
        "Recipes.MaxTips", "Recipe cannot have more than three tips.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed sub-ingredients (5) 
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxSubIngredients() => new(
        "Recipes.MaxSubIngredients", "Recipe cannot have more than five sub ingredients.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed ingredients (10)
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxIngredients() => new(
        "Recipes.MaxIngredients", "Recipe cannot have more than ten ingredients.");
}