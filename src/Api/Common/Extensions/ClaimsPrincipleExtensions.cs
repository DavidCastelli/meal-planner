using System.Security.Claims;

namespace Api.Common.Extensions;

/// <summary>
/// Extension methods for the <see cref="ClaimsPrincipal"/> class.
/// </summary>
internal static class ClaimsPrincipleExtensions
{
    /// <summary>
    /// Extension method used to get the user id from the user claims and convert it to an integer.
    /// </summary>
    /// <remarks>
    /// Attempts to parse and convert the user id from a string to the type matching a users primary key.
    /// </remarks>
    /// <param name="principal">The current users <see cref="ClaimsPrincipal"/>.</param>
    /// <returns>An integer representing the user's id.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the <paramref name="principal"/> is null, the claim for the user id is not found, or the user id failed to be parsed.</exception>
    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userId, out int parsedUserId)
            ? parsedUserId
            : throw new InvalidOperationException("User id is unavailable");
    }
}