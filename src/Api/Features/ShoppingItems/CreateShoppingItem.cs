using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Domain.ShoppingItems;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to create a shopping item.
/// </summary>
public sealed class CreateShoppingItemController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for shopping item creation.
    /// </summary>
    /// <param name="validator">The validator for the request.</param>
    /// <param name="request">The request.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpPost("/api/shopping-items")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, Created>> CreateAsync(
        IValidator<CreateShoppingItemRequest> validator, CreateShoppingItemRequest request, CreateShoppingItemHandler handler, CancellationToken cancellationToken)
    {
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var shoppingItemId = await handler.HandleAsync(request, cancellationToken);

        var location = Url.Link("GetShoppingItemById", new { id = shoppingItemId });
        return TypedResults.Created(location);
    }
}

/// <summary>
/// The request for shopping item creation.
/// </summary>
/// <param name="Name">The name of the shopping item.</param>
/// <param name="Measurement">The measurement of the shopping item.</param>
/// <param name="Price">The price of the shopping item.</param>
/// <param name="Quantity">The quantity of the shopping item.</param>
public sealed record CreateShoppingItemRequest(string Name, string? Measurement, decimal? Price, int? Quantity);

internal sealed class CreateShoppingItemRequestValidator : AbstractValidator<CreateShoppingItemRequest>
{
    public CreateShoppingItemRequestValidator()
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
/// The handler that handles a shopping item creation request.
/// </summary>
public sealed class CreateShoppingItemHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="CreateShoppingItemHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public CreateShoppingItemHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to create a shopping item.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns an integer specifying the id of the created shopping item.
    /// </returns>
    public async Task<int> HandleAsync(CreateShoppingItemRequest request, CancellationToken cancellationToken)
    {
        var shoppingItem = FromDto(request, _userContext.UserId);

        _dbContext.Add(shoppingItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return shoppingItem.Id;
    }

    private static ShoppingItem FromDto(CreateShoppingItemRequest request, int userId) => new()
    {
        Name = request.Name,
        Measurement = request.Measurement,
        Price = request.Price,
        Quantity = request.Quantity,
        ApplicationUserId = userId
    };
}