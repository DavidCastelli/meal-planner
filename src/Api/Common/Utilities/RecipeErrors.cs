using Api.Domain.Recipes;

namespace Api.Common.Utilities;

/// <summary>
/// Utility class used to retrieve <see cref="Error"/> messages in relation to a recipe.
/// </summary>
public static class RecipeErrors
{
    /// <summary>
    /// Constant defining the maximum number of recipes a user can create.
    /// </summary>
    public const int MaxRecipesCount = 50;

    /// <summary>
    /// Constant defining the maximum number of directions a recipe can contain.
    /// </summary>
    public const int MaxDirectionsCount = 6;

    /// <summary>
    /// Constant defining the maximum number of tips a recipe can contain.
    /// </summary>
    public const int MaxTipsCount = 3;

    /// <summary>
    /// Constant defining the maximum number of sub ingredients a recipe can contain.
    /// </summary>
    public const int MaxSubIngredientsCount = 5;

    /// <summary>
    /// Constant defining the maximum number of ingredients a sub ingredient can contain.
    /// </summary>
    public const int MaxIngredientsCount = 10;

    /// <summary>
    /// Error message that is used when a recipe's application user id does not match that of the current requests
    /// user id. 
    /// </summary>
    /// <remarks>
    /// This error message is mainly used when a user tries to create or update a meal and passes a recipe id which does not belong
    /// to them.
    /// </remarks>
    /// <param name="userId">The id of the user of the current request.</param>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error DoesNotBelongToUser(int userId) => new(
        "Recipes.DoesNotBelongToUser", $"One or more recipes do not belong to user: {userId}.");

    /// <summary>
    /// Error message that is used when a user tries to update a collection belonging to a recipe
    /// and passes an entity id which the recipe's collection does not already contain.
    /// </summary>
    /// <param name="recipeId">The id of the recipe.</param>
    /// <param name="entityId">The id of the entity which the recipe's collection did not contain.</param>
    /// <param name="entityType">The type of the entity collection.</param>
    /// <returns></returns>
    public static Error DoesNotBelongToRecipe(int recipeId, int entityId, string entityType) => new(
        "Recipes.DoesNotBelongToRecipe",
        $"Entity of type {entityType} with id: {entityId} does not belong to recipe with id: {recipeId}.");

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
    /// Error message that is used when a recipe contains a single sub ingredient and this sub ingredient is provided an name.
    /// </summary>
    /// <remarks>
    /// For more information on when to use the error see <see cref="SubIngredient"/>
    /// </remarks>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error SingleSubIngredientName() => new(
        "Recipes.SingleSubIngredientName", "Sub ingredient must not contain a name when only one sub ingredient is provided.");

    /// <summary>
    /// Error message that is used when a recipe contains multiple sub ingredients but at least one of which is not
    /// given a name.
    /// </summary>
    /// <remarks>
    /// For more information on when to use the error see <see cref="SubIngredient"/>
    /// </remarks>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MultipleSubIngredientName() => new(
        "Recipes.MultipleSubIngredientName", "All sub ingredients must have a name when more than one is provided.");

    /// <summary>
    /// Error message that is used when a user attempts to create a recipe and exceeds the maximum number permitted <see cref="MaxRecipesCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxRecipes() => new Error(
        "Recipes.MaxRecipes", $"Cannot create more than the maximum number of recipes: {MaxRecipesCount}.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed directions <see cref="MaxDirectionsCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxDirections() => new(
    "Recipes.MaxDirections", $"Recipe cannot have more than {MaxDirectionsCount} directions.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed tips <see cref="MaxTipsCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxTips() => new(
        "Recipes.MaxTips", $"Recipe cannot have more than {MaxTipsCount} tips.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed sub-ingredients <see cref="MaxSubIngredientsCount"/>. 
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxSubIngredients() => new(
        "Recipes.MaxSubIngredients", $"Recipe cannot have more than {MaxSubIngredientsCount} sub ingredients.");

    /// <summary>
    /// Error message that is used when a recipe contains more than the maximum number of allowed ingredients <see cref="MaxIngredientsCount"/>.
    /// </summary>
    /// <returns>An <see cref="Error"/> which contains an error code and description.</returns>
    public static Error MaxIngredients() => new(
        "Recipes.MaxIngredients", $"Recipe cannot have more than {MaxIngredientsCount} ingredients.");
}