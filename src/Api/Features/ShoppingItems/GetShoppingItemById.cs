using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to get a shopping item by id.
/// </summary>
public sealed class GetShoppingItemByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting a shopping item by id.
    /// </summary>
    /// <param name="id">The id of the shopping item to get.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpGet("/api/shopping-items/{id:int}", Name = "GetShoppingItemById")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetShoppingItemByIdDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<GetShoppingItemByIdDto>>> GetByIdAsync(int id,
        GetShoppingItemByIdHandler handler, CancellationToken cancellationToken)
    {
        var getShoppingItemByIdDto = await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok(getShoppingItemByIdDto);
    }
}

/// <summary>
/// The handler that handles getting a shopping item by id.
/// </summary>
public sealed class GetShoppingItemByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetShoppingItemByIdHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetShoppingItemByIdHandler(MealPlannerContext dbContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to get a shopping item by id.
    /// </summary>
    /// <param name="id">The id of the shopping item to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="GetShoppingItemByIdDto"/>.
    /// </returns>
    /// <exception cref="ShoppingItemNotFoundException">Is thrown if the shopping item could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<GetShoppingItemByIdDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var shoppingItem = await _dbContext.ShoppingItem
            .Where(si => si.Id == id)
            .Select(si => new GetShoppingItemByIdDto(si.Id, si.Name, si.Measurement, si.Price, si.Quantity,
                si.IsChecked, si.IsLocked, si.IsGenerated, si.ApplicationUserId))
            .SingleOrDefaultAsync(cancellationToken);

        if (shoppingItem == null)
        {
            throw new ShoppingItemNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, shoppingItem, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return shoppingItem;
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

/// <summary>
/// The DTO to return to the client after getting a shopping item by id.
/// </summary>
/// <param name="Id">The id of the shopping item.</param>
/// <param name="Name">The name of the shopping item.</param>
/// <param name="Measurement">The measurement of the shopping item.</param>
/// <param name="Price">The price of the shopping item.</param>
/// <param name="Quantity">The quantity of the shopping item.</param>
/// <param name="IsChecked">If the shopping item is checked.</param>
/// <param name="IsLocked">If the shopping item is locked.</param>
/// <param name="IsGenerated">If the shopping item is generated.</param>
/// <param name="ApplicationUserId">The application user id of the user who the shopping item belongs to.</param>
public sealed record GetShoppingItemByIdDto(int Id, string Name, string Measurement, decimal? Price, int? Quantity, bool IsChecked, bool IsLocked, bool IsGenerated, int ApplicationUserId) : IAuthorizable;