using System.Security.Claims;

using Api.Common.Extensions;
using Api.Common.Interfaces;

namespace Api.Infrastructure.Services;

/// <summary>
/// A class implementing <see cref="IUserContext"/>.
/// </summary>
/// <remarks>
/// The implementation of this class relies on the <see cref="IHttpContextAccessor"/> whose use can be dangerous.
/// The <see cref="UserContext"/> should only be used in the scope of a request where the http context will be available.
/// </remarks>
internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Creates a <see cref="UserContext"/>.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor.</param>
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's id.
    /// </summary>
    /// <exception cref="InvalidOperationException">Is thrown if the http context is not available.</exception>
    public int UserId =>
        _httpContextAccessor.
            HttpContext?
            .User
            .GetUserId() ??
        throw new InvalidOperationException("User context is unavailable");

    /// <summary>
    /// Gets the current user's authentication status.
    /// </summary>
    /// <exception cref="InvalidOperationException">Is thrown if the http context is not available.</exception>
    public bool IsAuthenticated =>
        _httpContextAccessor
            .HttpContext?
            .User
            .Identity?
            .IsAuthenticated ??
        throw new InvalidOperationException("User context is unavailable");

    /// <summary>
    /// Gets the current user, who represented by a <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Is thrown if the http context is not available.</exception>
    public ClaimsPrincipal User =>
        _httpContextAccessor.
            HttpContext?
            .User ??
        throw new InvalidOperationException("User context is unavailable");
}