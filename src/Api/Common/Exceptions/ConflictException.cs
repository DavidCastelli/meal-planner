namespace Api.Common.Exceptions;

/// <summary>
/// Base exception for conflict.
/// </summary>
/// <remarks>
/// All exceptions derived from <see cref="ConflictException"/> get mapped by <see cref="ExceptionHandler"/> into a problem details with
/// status 409 before being returned to the client.
/// </remarks>
public abstract class ConflictException : Exception
{
    /// <summary>
    /// Creates a <see cref="ConflictException"/> given a <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The error message of the exception.</param>
    protected ConflictException(string message)
        : base(message)
    {
    }
}