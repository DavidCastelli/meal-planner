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
    /// This action method by default sets the schedule property of all scheduled meals belonging to the current user to Schedule.None.
    /// </remarks>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpPatch("/api/manage/meals/schedule")]
    [Consumes(MediaTypeNames.Application.JsonPatch)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, Ok>> PatchAsync(
         ClearScheduledMealsHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(cancellationToken);

        return TypedResults.Ok();
    }
}

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
        await _dbContext.Meal
            .Where(m => m.Schedule != Schedule.None && m.ApplicationUserId == _userContext.UserId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.Schedule, Schedule.None), cancellationToken);
    }
}