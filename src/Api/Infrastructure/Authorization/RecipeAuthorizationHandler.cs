using Api.Common.Extensions;
using Api.Domain.Recipes;

using Microsoft.AspNetCore.Authorization;

namespace Api.Infrastructure.Authorization;

/// <summary>
/// A recipe authorization handler which determines if a user has permission to read or modify a recipe.
/// </summary>
public sealed class RecipeAuthorizationHandler : AuthorizationHandler<SameUserRequirement, Recipe>
{
    /// <summary>
    /// Makes a decisions on if a user can read or modify a <see cref="Recipe"/>.
    /// </summary>
    /// <remarks>
    /// The decision is made based on if the current user id matches that of the recipes application user id.
    /// </remarks>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    /// <param name="resource">The resource to evaluate.</param>
    /// <returns>A task which signals the status of the asynchronous operation.</returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement,
        Recipe resource)
    {
        if (context.User.GetUserId() == resource.ApplicationUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}