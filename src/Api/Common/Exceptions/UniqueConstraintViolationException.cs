namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a unique constraint violation occurs at the database.
/// </summary>
/// <remarks>
/// This exception is mapped by <see cref="ExceptionHandler"/> into a problem details with status 409 before being returned to the client.
/// </remarks>
public sealed class UniqueConstraintViolationException : Exception
{
    /// <summary>
    /// Creates a <see cref="UniqueConstraintViolationException"/> given a <paramref name="constraintName"/>.
    /// </summary>
    /// <param name="constraintName">The name of the property which violated the unique constraint.</param>
    public UniqueConstraintViolationException(string? constraintName)
        : base(constraintName == null ? "A unique constraint violation has occured." : $"{constraintName} must be unique.")
    {
    }
}