namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a unique constraint violation occurs at the database.
/// </summary>
public sealed class UniqueConstraintViolationException : ConflictException
{
    /// <summary>
    /// Creates a <see cref="UniqueConstraintViolationException"/> given a <paramref name="cause"/>.
    /// </summary>
    /// <param name="cause">The cause for the unique constraint violation.</param>
    public UniqueConstraintViolationException(string? cause)
        : base(cause == null ? "A unique constraint violation has occured." : $"{cause} must be unique.")
    {
    }
}