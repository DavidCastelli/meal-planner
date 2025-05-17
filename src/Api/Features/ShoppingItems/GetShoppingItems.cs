using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to get all shopping items belonging to the current user.
/// </summary>
public sealed class GetShoppingItemsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting all shopping items belonging to the current user.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpGet("/api/shopping-items", Name = "GetShoppingItems")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<GetShoppingItemsDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, Ok<IEnumerable<GetShoppingItemsDto>>>> GetAsync(
        GetShoppingItemsHandler handler, CancellationToken cancellationToken)
    {
        var getShoppingItemsDtos = await handler.HandleAsync(cancellationToken);

        return TypedResults.Ok(getShoppingItemsDtos);
    }
}

/// <summary>
/// The handler that handles getting all shopping items belonging to the current user.
/// </summary>
public sealed class GetShoppingItemsHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="GetShoppingItemsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public GetShoppingItemsHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to get all shopping items belonging to the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns an enumerable <see cref="GetShoppingItemsDto"/>.
    /// </returns>
    public async Task<IEnumerable<GetShoppingItemsDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var shoppingItems = await _dbContext.ShoppingItem
            .Where(si => si.ApplicationUserId == _userContext.UserId)
            .Select(si => new GetShoppingItemsDto(si.Id, si.Name, si.Measurement, si.Price, si.Quantity, si.IsChecked,
                si.IsLocked, si.IsGenerated))
            .ToListAsync(cancellationToken);

        return shoppingItems;
    }
}

/// <summary>
/// The DTO to return to the client after getting all shopping items belonging to the current user.
/// </summary>
/// <param name="Id">The id of the shopping item.</param>
/// <param name="Name">The name of the shopping item.</param>
/// <param name="Measurement">The measurement of the shopping item.</param>
/// <param name="Price">The price of the shopping item.</param>
/// <param name="Quantity">The quantity of the shopping item.</param>
/// <param name="IsChecked">If the shopping item is checked.</param>
/// <param name="IsLocked">If the shopping item is locked.</param>
/// <param name="IsGenerated">If the shopping item is generated.</param>
public sealed record GetShoppingItemsDto(int Id, string Name, string? Measurement, decimal? Price, int? Quantity, bool IsChecked, bool IsLocked, bool IsGenerated);