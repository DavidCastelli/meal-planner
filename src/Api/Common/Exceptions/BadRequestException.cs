using Api.Domain;

namespace Api.Common.Exceptions;

/// <summary>
/// Base exception for bad requests.
/// </summary>
/// <remarks>
/// All exceptions derived from <see cref="BadRequestException"/> get mapped by <see cref="ExceptionHandler"/> into a problem details with
/// status code 400 before being returned to the client. The problem details also includes a list of <see cref="Error"/> messages which
/// describe what went wrong for the client.
/// </remarks>
public abstract class BadRequestException : Exception
{
    /// <summary>
    /// Gets a list of <see cref="Error"/> messages.
    /// </summary>
    /// <value>
    /// An array of errors.
    /// </value>
    public Error[] Errors { get; }

    /// <summary>
    /// Creates a <see cref="BadRequestException"/> given a <paramref name="message"/> and list of <paramref name="errors"/>.
    /// </summary>
    /// <param name="message">The error message of the exception.</param>
    /// <param name="errors">Array of <see cref="Error"/> messages for the client.</param>
    protected BadRequestException(string message, Error[]? errors = null)
        : base(message)
    {
        Errors = errors ?? [];
    }
}