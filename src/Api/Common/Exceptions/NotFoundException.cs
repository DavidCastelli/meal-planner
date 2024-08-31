namespace Api.Common.Exceptions;

/// <summary>
/// Base exception for not found.
/// </summary>
/// <remarks>
/// All exceptions derived from <see cref="NotFoundException"/> get mapped by <see cref="ExceptionHandler"/> into a problem details with
/// status 404 before being returned to the client.
/// </remarks>
public abstract class NotFoundException : Exception
{
    /// <summary>
    /// Creates a <see cref="NotFoundException"/> given a <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The error message of the exception.</param>
    protected NotFoundException(string message)
        : base(message)
    {
    }
}