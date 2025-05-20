using System.Net.Mime;
using System.Text;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Common.Utilities;
using Api.Domain.Meals;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Meals;

/// <summary>
/// Controller that handles requests to patch the schedule of a meal.
/// </summary>
public sealed class ScheduleMealByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for patching the schedule of a meal.
    /// </summary>
    /// <remarks>
    /// This action method only supports the add and remove operations when patching.
    /// Further, it only supports a single operation at a time and may patch only the schedule property of a meal.
    /// </remarks>
    /// <param name="id">The id of the meal to patch.</param>
    /// <param name="document">The patch document.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}"/> object.
    /// </returns>
    [HttpPatch("/api/manage/meals/{id:int}/schedule")]
    [Consumes(MediaTypeNames.Application.JsonPatch)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Meals")]
    public async
        Task<Results<UnauthorizedHttpResult, UnprocessableEntity<ProblemDetails>, ValidationProblem, NotFound, ForbidHttpResult, Ok>> PatchAsync(
            int id, JsonPatchDocument<ScheduleMealByIdRequest> document, ScheduleMealByIdHandler handler, CancellationToken cancellationToken)
    {
        if (document.Operations.Count != 1)
        {
            var problemDetails = new ProblemDetails { Detail = "Only one operation is allowed" };
            return TypedResults.UnprocessableEntity(problemDetails);
        }

        if (document.Operations[0].OperationType != OperationType.Add && document.Operations[0].OperationType != OperationType.Remove)
        {
            var problemDetails = new ProblemDetails { Detail = "Only the add or remove operation is allowed." };
            return TypedResults.UnprocessableEntity(problemDetails);
        }

        var request = new ScheduleMealByIdRequest(Schedule.None);
        document.ApplyTo(request, ModelState);

        if (!ModelState.IsValid)
        {
            var validationProblemDetails = new ValidationProblemDetails(ModelState);
            return TypedResults.ValidationProblem(
                validationProblemDetails.Errors,
                validationProblemDetails.Detail,
                validationProblemDetails.Instance,
                validationProblemDetails.Title,
                validationProblemDetails.Type,
                validationProblemDetails.Extensions);
        }

        await handler.HandleAsync(id, request, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The request to patch the schedule of a meal.
/// </summary>
/// <param name="Schedule">The schedule of the meal to patch.</param>
public sealed record ScheduleMealByIdRequest(Schedule Schedule);

/// <summary>
/// The handler that handles a meal schedule patch request.
/// </summary>
public sealed class ScheduleMealByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="ScheduleMealByIdHandler"/>.
    /// </summary>
    /// <param name="dContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public ScheduleMealByIdHandler(MealPlannerContext dContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to patch the schedule of a meal.
    /// </summary>
    /// <param name="id">The id of the meal to patch.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="MealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(int id, ScheduleMealByIdRequest request,
        CancellationToken cancellationToken)
    {
        var meal = await _dbContext.Meal
            .FindAsync([id], cancellationToken: cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            if (meal.Schedule == request.Schedule)
            {
                return;
            }

            if (request.Schedule == Schedule.None)
            {
                meal.Schedule = Schedule.None;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return;
            }

            var scheduledCount = _dbContext.Meal
                .Count(m => m.Schedule == request.Schedule && m.ApplicationUserId == _userContext.UserId);

            var validationErrors = ValidateMeal(scheduledCount);

            if (validationErrors.Length != 0)
            {
                throw new MealValidationException(validationErrors);
            }

            meal.Schedule = request.Schedule;

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

    private static Error[] ValidateMeal(int scheduledCount)
    {
        List<Error> errors = [];

        if (scheduledCount + 1 > MealErrors.MaxScheduleCount)
        {
            errors.Add(MealErrors.MaxScheduled());
        }

        return [.. errors];
    }
}