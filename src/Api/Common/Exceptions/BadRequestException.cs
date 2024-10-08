using Api.Domain;

using Microsoft.AspNetCore.Mvc;

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

    /// <summary>
    /// Converts the list of <see cref="Errors"/> into a dictionary.
    /// </summary>
    /// <remarks>
    /// Used to convert the <see cref="Errors"/> property into a dictionary compatible with the errors property of a <see cref="ValidationProblemDetails"/>
    /// </remarks>
    /// <returns>A <see cref="Dictionary{TKey,TValue}"/> where TKey is a string error code and TValue is a string array of error messages belonging to TKey.</returns>
    public Dictionary<string, string[]> ToDictionary()
    {
        var dictionary = new Dictionary<string, string[]>();

        foreach (var error in Errors)
        {
            if (dictionary.TryGetValue(error.Code, out string[]? errors))
            {
                dictionary[error.Code] = [.. errors, error.Description];
            }
            else
            {
                dictionary.Add(error.Code, [error.Description]);
            }
        }
        
        return dictionary;
    }
}