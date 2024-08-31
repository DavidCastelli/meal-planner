using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.RecipeManagement;

/// <summary>
/// Controller that handles requests to delete a recipe.
/// </summary>
public sealed class DeleteRecipeController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for deleting a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe to be deleted.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpDelete("/api/manage/recipes/{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok>> DeleteAsync(int id, DeleteRecipeHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The handler that handles a recipe deletion request.
/// </summary>
public sealed class DeleteRecipeHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="DeleteRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public DeleteRecipeHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to delete a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipes.FindAsync([id], cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            _dbContext.Recipes.Remove(recipe);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (_userContext.IsAuthenticated)
        {
            throw new ForbiddenException();
        }
        else
        {
            throw new UnauthorizedException();
        }
    }
}