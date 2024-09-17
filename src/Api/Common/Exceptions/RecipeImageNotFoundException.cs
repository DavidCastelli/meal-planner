namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when an image could not be found for a recipe.
/// </summary>
public sealed class RecipeImageNotFoundException : NotFoundException
{
    /// <summary>
    /// Creates a <see cref="RecipeImageNotFoundException"/> given a <paramref name="recipeId"/>.
    /// </summary>
    /// <param name="recipeId">The id of the recipe whose image could not be found.</param>
    public RecipeImageNotFoundException(int recipeId)
        : base($"No image found for recipe with id: {recipeId}.")
    {

    }
}