using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Common.Utilities;
using Api.Domain.ShoppingItems;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to update a shopping item.
/// </summary>
public sealed class UpdateShoppingItemController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for updating a shopping item.
    /// </summary>
    /// <param name="id">The id of the shopping item to update.</param>
    /// <param name="validator">The validator for the request.</param>
    /// <param name="request">The request.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}"/> object.
    /// </returns>
    [HttpPut("/api/shopping-items/{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, NotFound, Conflict, ForbidHttpResult, Ok>> UpdateAsync(int id,
        IValidator<UpdateShoppingItemRequest> validator, UpdateShoppingItemRequest request, UpdateShoppingItemHandler handler,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Request.InvalidId", ["The request id must match the route id."] }
            };
            return TypedResults.ValidationProblem(errors);
        }

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        await handler.HandleAsync(request, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The request to update a shopping item.
/// </summary>
/// <param name="Id">The id of the shopping item to update.</param>
/// <param name="Name">The name of the shopping item.</param>
/// <param name="Measurement">The measurement of the shopping item.</param>
/// <param name="Price">The price of the shopping item.</param>
/// <param name="Quantity">The quantity of the shopping item.</param>
/// <param name="IsChecked">If the shopping item is checked.</param>
/// <param name="IsLocked">If the shopping item is locked.</param>
public sealed record UpdateShoppingItemRequest(int Id, string Name, string? Measurement, decimal? Price, int? Quantity, bool IsChecked, bool IsLocked);

internal sealed class UpdateShoppingItemRequestValidator : AbstractValidator<UpdateShoppingItemRequest>
{
    public UpdateShoppingItemRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.Measurement)
            .MaximumLength(20)
            .When(si => si.Measurement != null);

        RuleFor(request => request.Price)
            .GreaterThanOrEqualTo(0)
            .PrecisionScale(12, 2, false)
            .When(si => si.Price != null);

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .When(si => si.Quantity != null);
    }
}

/// <summary>
/// The handler that handles a shopping item update request.
/// </summary>
public sealed class UpdateShoppingItemHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="UpdateShoppingItemHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public UpdateShoppingItemHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to update a shopping item.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="ShoppingItemNotFoundException">Is thrown if the shopping item could not be found in the data store.</exception>
    /// <exception cref="LockedGeneratedShoppingItemException">Is thrown if the user attempts to lock a shopping item that is marked as generated.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(UpdateShoppingItemRequest request, CancellationToken cancellationToken)
    {
        var newShoppingItem = FromDto(request, _userContext.UserId);

        var oldShoppingItem = await _dbContext.ShoppingItem.FindAsync([request.Id], cancellationToken);

        if (oldShoppingItem == null)
        {
            throw new ShoppingItemNotFoundException(request.Id);
        }

        if (oldShoppingItem.IsGenerated && newShoppingItem.IsLocked)
        {
            throw new LockedGeneratedShoppingItemException(request.Id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, oldShoppingItem, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            _dbContext.Entry(oldShoppingItem).CurrentValues.SetValues(newShoppingItem);
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

    private static ShoppingItem FromDto(UpdateShoppingItemRequest request, int userId) => new()
    {
        Id = request.Id,
        Name = request.Name,
        Measurement = request.Measurement,
        Price = request.Price,
        Quantity = request.Quantity,
        IsChecked = request.IsChecked,
        IsLocked = request.IsLocked,
        ApplicationUserId = userId
    };
}