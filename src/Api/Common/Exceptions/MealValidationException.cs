namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when meal validation fails.
/// </summary>
public sealed class MealValidationException : BadRequestException
{
    /// <summary>
    /// Creates a <see cref="MealValidationException"/> given a list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    public MealValidationException(Error[] errors)
        : base("Meal validation failed", errors)
    {
    }
}