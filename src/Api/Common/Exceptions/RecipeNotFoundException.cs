namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a recipe could not be found when searched in the data store.
/// </summary>
public sealed class RecipeNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="RecipeNotFoundException"/> given a <paramref name="recipeId"/>.
    /// </summary>
    /// <param name="recipeId">The id of the recipe that could not be found.</param>
    public RecipeNotFoundException(int recipeId)
        : base($"Recipe with id: {recipeId} was not found")
    {
    }
}