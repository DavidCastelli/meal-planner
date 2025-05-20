using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to delete all shopping items belonging to the current user that are not locked.
/// </summary>
public sealed class ClearShoppingItemsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for deleting all shopping items belonging to the current user that are not locked.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <param name="clear">Query param that determines which shopping items to clear. Currently supported values are all and checked.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpDelete("/api/shopping-items")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, UnprocessableEntity<ProblemDetails>, Ok>> DeleteAllAsync(ClearShoppingItemsHandler handler,
        CancellationToken cancellationToken, string clear = "all")
    {
        if (string.IsNullOrEmpty(clear) || (clear != "all" && clear != "checked"))
        {
            var problemDetails = new ProblemDetails { Detail = "Query parameter clear must be set to one of the following values: all, checked." };
            return TypedResults.UnprocessableEntity(problemDetails);
        }

        await handler.HandleAsync(clear, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The handler that handles a delete all shopping items belonging to the current user that are not locked request.
/// </summary>
public sealed class ClearShoppingItemsHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="ClearShoppingItemsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public ClearShoppingItemsHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to delete all shopping items belonging to the current user that are not locked.
    /// </summary>
    /// <param name="clear">Determines which shopping items to clear. Currently supported values are all and checked.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    public async Task HandleAsync(string clear, CancellationToken cancellationToken)
    {
        if (clear == "all")
        {
            await _dbContext.ShoppingItem
                .Where(si => si.ApplicationUserId == _userContext.UserId && !si.IsLocked)
                .ExecuteDeleteAsync(cancellationToken);
        }
        else
        {
            await _dbContext.ShoppingItem
                .Where(si => si.ApplicationUserId == _userContext.UserId && !si.IsLocked && si.IsChecked)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}