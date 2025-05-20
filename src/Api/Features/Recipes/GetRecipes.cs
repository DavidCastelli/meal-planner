using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Recipes;

/// <summary>
/// Controller that handles requests to get all recipes belonging to the current user.
/// </summary>
public sealed class GetRecipesController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting all recipes belonging to the current user.
    /// </summary>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2}"/> object.
    /// </returns>
    [HttpGet("/api/manage/recipes")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(IEnumerable<GetRecipesDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, Ok<IEnumerable<GetRecipesDto>>>> GetAsync(GetRecipesHandler handler, CancellationToken cancellationToken)
    {
        var getRecipesDtos = await handler.HandleAsync(cancellationToken);

        return TypedResults.Ok(getRecipesDtos);
    }
}

/// <summary>
/// The handler that handles getting all recipes belonging to the current user.
/// </summary>
public sealed class GetRecipesHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="GetRecipesHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public GetRecipesHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to get all recipes belonging to the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="GetRecipesDto"/>.
    /// </returns>
    public async Task<IEnumerable<GetRecipesDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var recipes = await _dbContext.Recipe
            .Where(r => r.ApplicationUserId == _userContext.UserId)
            .Select(r =>
                new GetRecipesDto(r.Id, r.Title, r.Image == null ? null
                    : new GetRecipesImageDto(r.Image.Id, r.Image.StorageFileName, r.Image.DisplayFileName, r.Image.ImageUrl))
            )
            .ToListAsync(cancellationToken);

        return recipes;
    }
}

/// <summary>
/// The DTO to return to the client when getting all recipes belonging to the current user.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Image">The image of the recipe.</param>
public sealed record GetRecipesDto(int Id, string Title, GetRecipesImageDto? Image);

/// <summary>
/// The DTO for an image to return to the client when getting all recipes belonging to the current user.
/// </summary>
/// <param name="Id">The id of the image.</param>
/// <param name="StorageFileName">The storage file name of the image.</param>
/// <param name="DisplayFileName">The display file name of the image.</param>
/// <param name="ImageUrl">A valid URL for the image.</param>
public sealed record GetRecipesImageDto(int Id, string StorageFileName, string DisplayFileName, string ImageUrl);