using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain.Meals;
using Api.Domain.Tags;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MealManagement;

/// <summary>
/// Controller that handles requests to get a meal by id.
/// </summary>
public sealed class GetMealByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting a meal by id.
    /// </summary>
    /// <param name="id">The id of the meal to get.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpGet("/api/manage/meals/{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetByIdMealDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<GetByIdMealDto>>> GetByIdAsync(int id, GetMealByIdHandler handler, CancellationToken cancellationToken)
    {
        var getMealDto = await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok(getMealDto);
    }
}

/// <summary>
/// The handler that handles getting a meal by id.
/// </summary>
public sealed class GetMealByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetMealByIdHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetMealByIdHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to get a meal by id.
    /// </summary>
    /// <param name="id">The id of the meal to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="GetByIdMealDto"/>.
    /// </returns>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<GetByIdMealDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var meal = await _dbContext.Meals
            .Include(m => m.Tags)
            .Include(m => m.Recipes)
            .AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return ToDto(meal);
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

    private static GetByIdMealDto ToDto(Meal meal)
    {
        var tagDtos = new List<GetByIdTagDto>();

        foreach (var tag in meal.Tags)
        {
            var tagDto = new GetByIdTagDto(tag.Id, tag.Type);

            tagDtos.Add(tagDto);
        }

        var recipeDtos = new List<GetByIdMealRecipeDto>();

        foreach (var recipe in meal.Recipes)
        {
            var recipeDto = new GetByIdMealRecipeDto(recipe.Id, recipe.Title, recipe.Description);

            recipeDtos.Add(recipeDto);
        }

        return new GetByIdMealDto(meal.Id, meal.Title, tagDtos, recipeDtos, meal.ApplicationUserId);
    }
}

/// <summary>
/// The DTO to return to the client after getting a meal by id.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Tags">A collection of tags belonging to the meal.</param>
/// <param name="Recipes">A collection of recipes belonging to the meal.</param>
/// <param name="ApplicationUserId">The id of the user who the meal belongs to.</param>
public sealed record GetByIdMealDto(int Id, string Title, ICollection<GetByIdTagDto> Tags, ICollection<GetByIdMealRecipeDto> Recipes, int ApplicationUserId);

/// <summary>
/// The DTO for a tag to return to the client.
/// </summary>
/// <param name="Id">The id of the tag.</param>
/// <param name="TagType">The type of tag.</param>
public sealed record GetByIdTagDto(int Id, TagType TagType);

/// <summary>
/// The DTO for a recipe to return to the client.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
public sealed record GetByIdMealRecipeDto(int Id, string Title, string? Description);