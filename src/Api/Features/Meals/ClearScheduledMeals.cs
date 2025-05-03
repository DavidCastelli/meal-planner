using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Domain.Meals;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Meals;

/// <summary>
/// Controller that handles requests to clear the scheduled meals belonging to the current user.
/// </summary>
public sealed class ClearScheduledMealsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for clearing the scheduled meals belonging to the current user.
    /// </summary>
    /// <remarks>
    /// This action method only supports the remove operation when patching.
    /// Further, it only supports a single operation at a time and may patch only the schedule property of a meal.
    /// The patch is applied to all meals the current user owns which are currently scheduled.
    /// </remarks>
    /// <param name="document">The patch document.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpPatch("/api/manage/meals/schedule")]
    [Consumes(MediaTypeNames.Application.JsonPatch)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Text.Plain)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, UnprocessableEntity<string>, BadRequest<ValidationProblemDetails>, Ok>> PatchAsync(
        JsonPatchDocument<ClearScheduledMealsRequest> document, ClearScheduledMealsHandler handler, CancellationToken cancellationToken)
    {
        if (document.Operations.Count != 1)
        {
            return TypedResults.UnprocessableEntity("Only one operation is allowed.");
        }

        if (document.Operations[0].OperationType != OperationType.Remove)
        {
            return TypedResults.UnprocessableEntity("Only the remove operation is allowed.");
        }

        var request = new ClearScheduledMealsRequest(Schedule.None);
        document.ApplyTo(request, ModelState);

        if (!ModelState.IsValid)
        {
            var validationProblemDetails = new ValidationProblemDetails(ModelState);
            return TypedResults.BadRequest(validationProblemDetails);
        }
        
        await handler.HandleAsync(cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The request to clear the scheduled meals belonging to the current user.
/// </summary>
/// <param name="Schedule">The schedule of the meal to patch.</param>
public sealed record ClearScheduledMealsRequest(Schedule Schedule);

/// <summary>
/// The handler that handles clearings the scheduled meals belonging to the current user.
/// </summary>
public sealed class ClearScheduledMealsHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="ClearScheduledMealsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public ClearScheduledMealsHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to clear the schedule of all meals belonging to the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        var meals = await _dbContext.Meal
            .Where(m => m.Schedule != Schedule.None && m.ApplicationUserId == _userContext.UserId)
            .ToListAsync(cancellationToken);

        foreach (var meal in meals)
        {
            meal.Schedule = Schedule.None;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
