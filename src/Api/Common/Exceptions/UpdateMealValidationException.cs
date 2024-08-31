using Api.Domain;

namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when updating a meal and validation fails.
/// </summary>
public class UpdateMealValidationException : BadRequestException
{
    /// <summary>
    /// Creates a <see cref="UpdateMealValidationException"/> given a list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    public UpdateMealValidationException(Error[] errors)
        : base("Validation failed while updating a meal", errors)
    {
    }
}