using System.Security.Claims;

namespace Api.Common.Interfaces;

/// <summary>
/// Classes implementing this interface are able to retrieve information on the current user.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the id of the current user.
    /// </summary>
    /// <value>
    /// An integer specifying the id of the current user.
    /// </value>
    public int UserId { get; }

    /// <summary>
    /// Gets whether the current user is authenticated.
    /// </summary>
    /// <value>
    /// True if the current user is authenticated, false otherwise.
    /// </value>
    public bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <value>
    /// A claims principle which represents the current user.
    /// </value>
    public ClaimsPrincipal User { get; }
}