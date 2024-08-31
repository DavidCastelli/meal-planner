using Api.Domain;

namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when creating a meal and validation fails.
/// </summary>
public sealed class CreateMealValidationException : BadRequestException
{
    /// <summary>
    /// Creates a <see cref="CreateMealValidationException"/> given a list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    public CreateMealValidationException(Error[] errors)
        : base("Validation failed while creating a meal", errors)
    {
    }
}