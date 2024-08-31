namespace Api.Common.Exceptions;

/// <summary>
/// Exception which is thrown when a user is authenticated but does not have permission to access the resource.
/// </summary>
/// <remarks>
/// This exception is mapped into a problem details with status code 403 by <see cref="ExceptionHandler"/> before being returned to the client.
/// </remarks>
public sealed class ForbiddenException : Exception
{
    /// <summary>
    /// Create a <see cref="ForbiddenException"/>.
    /// </summary>
    public ForbiddenException()
        : base("You don't have permission to access this resource")
    {
    }
}