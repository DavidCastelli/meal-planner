using Api.Common.Extensions;
using Api.Domain;

using Microsoft.AspNetCore.Authorization;

namespace Api.Infrastructure.Authorization;

/// <summary>
/// An authorization handler which determines if a user has permission to access a resource.
/// </summary>
/// <remarks>
/// A user who is able to access a resource may perform any action on the resource such as create, read, update, or delete.
/// </remarks>
public sealed class AuthorizationHandler : AuthorizationHandler<SameUserRequirement, IAuthorizable>
{
    /// <summary>
    /// Makes a decisions on if a user can access an <see cref="IAuthorizable"/> resource.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    /// <param name="resource">The resource to evaluate.</param>
    /// <returns>A task which signals the status of the asynchronous operation.</returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement,
        IAuthorizable resource)
    {
        if (context.User.GetUserId() == resource.ApplicationUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}