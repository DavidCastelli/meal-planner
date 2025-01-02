using System.Net.Mime;

using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.RecipeManagement;

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
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
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
    private readonly IUrlGenerator _urlGenerator;

    /// <summary>
    /// Creates a <see cref="GetRecipesHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    public GetRecipesHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
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
        var recipes = await _dbContext.Recipes
            .AsNoTracking()
            .Where(r => r.ApplicationUserId == _userContext.UserId)
            .Select(r =>
                new GetRecipesDto(r.Id, r.Title, r.ImagePath == null ? null :
                    $"{_urlGenerator.GenerateUrl("GetRecipeImageById", new { id = r.Id })}?refresh={Path.GetFileName(r.ImagePath)}") // Query param needed to refresh browser cache.
            )
            .ToListAsync(cancellationToken);

        return recipes;
    }
}

/// <summary>
/// The DTO to return to the client after getting all recipes belonging to the current user.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="ImageUrl">The URL of the recipes image.</param>
public sealed record GetRecipesDto(int Id, string Title, string? ImageUrl);