using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to delete a shopping item.
/// </summary>
public sealed class DeleteShoppingItemController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for shopping item deletion.
    /// </summary>
    /// <param name="id">The id of the shopping item to be deleted.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpDelete("/api/shopping-items/{id:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, ForbidHttpResult, Ok>> DeleteAsync(
        int id, DeleteShoppingItemHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The handler that handles a shopping item deletion request.
/// </summary>
public sealed class DeleteShoppingItemHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="DeleteShoppingItemHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public DeleteShoppingItemHandler(MealPlannerContext dbContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to delete a shopping item.
    /// </summary>
    /// <param name="id">The id of the shopping item to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="ShoppingItemNotFoundException">Is thrown if the shopping item could not be found in the data store.</exception>
    /// <exception cref="ShoppingItemLockedException">Is thrown if the user attempts to delete a shopping item which is marked as locked.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedAccessException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var shoppingItem = await _dbContext.ShoppingItem.FindAsync([id], cancellationToken);

        if (shoppingItem == null)
        {
            throw new ShoppingItemNotFoundException(id);
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(_userContext.User, shoppingItem, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            if (shoppingItem.IsLocked)
            {
                throw new ShoppingItemLockedException(shoppingItem.Id);
            }

            _dbContext.ShoppingItem.Remove(shoppingItem);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (_userContext.IsAuthenticated)
        {
            throw new ForbiddenException();
        }
        else
        {
            throw new UnauthorizedAccessException();
        }
    }
}