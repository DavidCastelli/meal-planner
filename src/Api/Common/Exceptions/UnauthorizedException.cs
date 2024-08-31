namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a user tries to access a resource which they lack the proper authentication credentials.
/// </summary>
/// <remarks>
/// This exception is mapped into a problem details with status code 401 by <see cref="ExceptionHandler"/> before being returned to the client.
/// </remarks>
public sealed class UnauthorizedException : Exception
{
    /// <summary>
    /// Creates a <see cref="UnauthorizedException"/>.
    /// </summary>
    public UnauthorizedException()
        : base("The request was not successful because it lacks valid authentication credentials for the requested resource")
    {
    }
}