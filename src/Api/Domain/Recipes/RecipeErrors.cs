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
    /// Error message that is used when a direction number is not unique for a given recipe.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DirectionNumberNotUnique() => new(
        "Recipes.DirectionNotUnique", $"One or more recipe direction numbers are not unique.");

    /// <summary>
    /// Error message that is used when a recipe contains multiple sub ingredients but at least one of which is not
    /// given a name.
    /// </summary>
    /// <remarks>
    /// For more information on when to use the error <see cref="SubIngredient"/>
    /// </remarks>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MultipleSubIngredientName() => new(
        "Recipes.MultipleSubIngredientName", $"All sub ingredients must have a name when more than one is provided");
}