using Microsoft.AspNetCore.Authorization;

namespace Api.Infrastructure.Authorization;

/// <summary>
/// An authorization requirement which requires the current user to match that of the owner of the resource trying to be accessed.
/// </summary>
public sealed class SameUserRequirement : IAuthorizationRequirement
{
}