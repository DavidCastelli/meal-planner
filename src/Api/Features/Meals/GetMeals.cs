using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Domain.Meals;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Meals;

/// <summary>
/// Controller that handles requests to get all meals belonging to the current user.
/// </summary>
public sealed class GetMealsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting all meals belonging to the current user.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <param name="scheduled">
    /// Optional query parameter which filters results to show only meals belonging to the current user which have been schedule to a specific day of the week.
    /// Is false by default.
    /// </param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpGet("/api/manage/meals")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(IEnumerable<GetMealsDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, Ok<IEnumerable<GetMealsDto>>>> GetAsync(GetMealsHandler handler, CancellationToken cancellationToken, bool scheduled = false)
    {
        var getMealsDtos = await handler.HandleAsync(scheduled, cancellationToken);

        return TypedResults.Ok(getMealsDtos);
    }
}

/// <summary>
/// The handler that handles getting all meals belonging to the current user.
/// </summary>
public sealed class GetMealsHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="GetMealsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public GetMealsHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to get all meals belonging to the current user.
    /// </summary>
    /// <param name="scheduled">Flag which determines if results are filtered to show only meals belonging to the current user which have been schedule to a specific day of the week.</param>
    /// <param name="cancellationToken">The cancellation token of the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns an enumerable <see cref="GetMealsDto"/>.
    /// </returns>
    public async Task<IEnumerable<GetMealsDto>> HandleAsync(bool scheduled, CancellationToken cancellationToken)
    {
        var meals = await _dbContext.Meal
            .Where(m => m.ApplicationUserId == _userContext.UserId)
            .Select(m =>
                new GetMealsDto(m.Id, m.Title, m.Image == null ? null
                    : new GetMealsImageDto(m.Image.Id, m.Image.StorageFileName, m.Image.DisplayFileName, m.Image.ImageUrl), m.Schedule)
            )
            .ToListAsync(cancellationToken);

        if (scheduled)
        {
            meals = meals.Where(m => m.Schedule != Schedule.None).ToList();
        }

        return meals;
    }
}

/// <summary>
/// The DTO to return to the client after getting all meals belonging to the current user.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">The image of the meal.</param>
/// <param name="Schedule">The day of the week on which the meal is scheduled.</param>
public sealed record GetMealsDto(int Id, string Title, GetMealsImageDto? Image, Schedule Schedule);

/// <summary>
/// The DTO for an image to return to the client when getting all meals belonging to the current user.
/// </summary>
/// <param name="Id">The id of the image.</param>
/// <param name="StorageFileName">The storage file name of the image.</param>
/// <param name="DisplayFileName">The display file name of the image.</param>
/// <param name="ImageUrl">A valid URL for the image.</param>
public sealed record GetMealsImageDto(int Id, string StorageFileName, string DisplayFileName, string ImageUrl);