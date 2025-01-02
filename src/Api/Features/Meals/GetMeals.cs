using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;
using Api.Infrastructure.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MealManagement;

/// <summary>
/// Controller that handles requests to get all meals belonging to the current user.
/// </summary>
public sealed class GetMealsController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting all meals belonging to the current user.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpGet("/api/manage/meals")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<GetMealsDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, Ok<IEnumerable<GetMealsDto>>>> GetAsync(GetMealsHandler handler, CancellationToken cancellationToken)
    {
        var getMealsDtos = await handler.HandleAsync(cancellationToken);

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
    private readonly IUrlGenerator _urlGenerator;

    /// <summary>
    /// Creates a <see cref="GetMealsHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    public GetMealsHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
    }

    /// <summary>
    /// Handles the database operations necessary to get all meals belonging to the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token of the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns an enumerable <see cref="GetMealsDto"/>.
    /// </returns>
    public async Task<IEnumerable<GetMealsDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var meals = await _dbContext.Meals
            .AsNoTracking()
            .Where(m => m.ApplicationUserId == _userContext.UserId)
            .Select(m =>
                new GetMealsDto(m.Id, m.Title, m.ImagePath == null ? null 
                    : $"{_urlGenerator.GenerateUrl("GetMealImageById", new { id = m.Id })}?refresh={Path.GetFileName(m.ImagePath)}") // Query param needed to refresh browser cache.
            )
            .ToListAsync(cancellationToken);

        return meals;
    }
}

/// <summary>
/// The DTO to return to the client after getting all meals belonging to the current user.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="ImageUrl">The URL of the meals image.</param>
public sealed record GetMealsDto(int Id, string Title, string? ImageUrl);