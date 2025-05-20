using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain.Meals;
using Api.Domain.ShoppingItems;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.ShoppingItems;

/// <summary>
/// Controller that handles requests to generate shopping items for the list of scheduled ingredients.
/// </summary>
public sealed class GenerateShoppingItemsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for generating shopping items for the list of scheduled ingredients.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpPost("/api/shopping-items/generate")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [Tags("Shopping Items")]
    public async Task<Results<UnauthorizedHttpResult, Conflict, Created>> GenerateAsync(
        GenerateShoppingItemsHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(cancellationToken);

        var location = Url.Link("GetShoppingItems", null);
        return TypedResults.Created(location);
    }
}

/// <summary>
/// The handler that handles a generate shopping items request.
/// </summary>
public sealed class GenerateShoppingItemsHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="GenerateShoppingItemsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public GenerateShoppingItemsHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to generate shopping items for the list of scheduled ingredients.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="GenerateUnclearedShoppingItemsException">Is thrown if the user attempts to generate a new shopping list before clearing the previously generated one.</exception>
    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        var generatedShoppingItemsCount = await _dbContext.ShoppingItem
            .CountAsync(si => si.ApplicationUserId == _userContext.UserId && si.IsGenerated, cancellationToken);

        if (generatedShoppingItemsCount != 0)
        {
            throw new GenerateUnclearedShoppingItemsException();
        }

        var meals = await _dbContext.Meal
            .Include(m => m.Recipes)
            .Where(m => m.Schedule != Schedule.None && m.ApplicationUserId == _userContext.UserId)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var shoppingItems = new List<ShoppingItem>();
        var ingredients = meals.SelectMany(m => m.Recipes)
            .SelectMany(r => r.SubIngredients)
            .SelectMany(si => si.Ingredients);

        foreach (var ingredient in ingredients)
        {
            shoppingItems.Add(new ShoppingItem { Name = ingredient.Name, Measurement = ingredient.Measurement, IsGenerated = true, ApplicationUserId = _userContext.UserId });
        }

        _dbContext.AddRange(shoppingItems);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}