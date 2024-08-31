using Api.Domain;

namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when recipe validation fails.
/// </summary>
public sealed class RecipeValidationException : BadRequestException
{
    /// <summary>
    /// Creates a <see cref="RecipeValidationException"/> given a list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    public RecipeValidationException(Error[] errors)
        : base("Recipe validation failed", errors)
    {
    }
}