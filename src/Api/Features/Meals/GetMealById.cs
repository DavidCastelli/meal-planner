using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.Tags;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Meals;

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
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpGet("/api/manage/meals/{id:int}", Name = "GetMealById")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(GetMealByIdDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, ForbidHttpResult, Ok<GetMealByIdDto>>> GetByIdAsync(int id, GetMealByIdHandler handler, CancellationToken cancellationToken)
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
    /// The result of the task upon completion returns a <see cref="GetMealByIdDto"/>.
    /// </returns>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<GetMealByIdDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var meal = await _dbContext.Meal
            .Where(m => m.Id == id)
            .Select(m => new GetMealByIdDto(
                m.Id,
                m.Title,
                m.Image == null ? null : new GetMealByIdImageDto(m.Image.Id, m.Image.StorageFileName, m.Image.DisplayFileName, m.Image.ImageUrl),
                m.Schedule,
                m.Tags.Select(t => new GetMealByIdTagDto(t.Id, t.Type)).ToList(),
                m.Recipes.Select(r => new GetMealByIdRecipeDto(
                    r.Id, r.Title, r.Description, r.Image == null ? null : r.Image.ImageUrl,
                    new GetMealByIdRecipeDetailsDto(r.RecipeDetails.PrepTime, r.RecipeDetails.CookTime, r.RecipeDetails.Servings),
                    new GetMealByIdRecipeNutritionDto(r.RecipeNutrition.Calories, r.RecipeNutrition.Fat, r.RecipeNutrition.Carbs, r.RecipeNutrition.Protein))).ToList(),
                m.ApplicationUserId))
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return meal;
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
}

/// <summary>
/// The DTO to return to the client after getting a meal by id.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">The image of the meal.</param>
/// <param name="Schedule">The day of the week on which the meal is scheduled.</param>
/// <param name="Tags">A collection of tags belonging to the meal.</param>
/// <param name="Recipes">A collection of recipes belonging to the meal.</param>
/// <param name="ApplicationUserId">The application user id of the user who the meal belongs to.</param>
public sealed record GetMealByIdDto(int Id, string Title, GetMealByIdImageDto? Image, Schedule Schedule, ICollection<GetMealByIdTagDto> Tags, ICollection<GetMealByIdRecipeDto> Recipes, int ApplicationUserId) : IAuthorizable;

/// <summary>
/// The DTO for an image to return to the client when getting a meal by id.
/// </summary>
/// <param name="Id">The id of the image.</param>
/// <param name="StorageFileName">The storage file name of the image.</param>
/// <param name="DisplayFileName">The display file name of the image.</param>
/// <param name="ImageUrl">A valid URL for the image.</param>
public sealed record GetMealByIdImageDto(int Id, string StorageFileName, string DisplayFileName, string ImageUrl);

/// <summary>
/// The DTO for a tag to return to the client when getting a meal by id.
/// </summary>
/// <param name="Id">The id of the tag.</param>
/// <param name="TagType">The type of tag.</param>
public sealed record GetMealByIdTagDto(int Id, TagType TagType);

/// <summary>
/// The DTO for a recipe to return to the client when getting a meal by id.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="ImageUrl">A valid URL for the recipe image.</param>
/// <param name="Details">The recipe details.</param>
/// <param name="Nutrition">The recipe nutrition.</param>
public sealed record GetMealByIdRecipeDto(int Id, string Title, string? Description, string? ImageUrl,
    GetMealByIdRecipeDetailsDto Details, GetMealByIdRecipeNutritionDto Nutrition);

/// <summary>
/// The DTO for the recipe details to return to the client when getting a meal by id.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record GetMealByIdRecipeDetailsDto(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record GetMealByIdRecipeNutritionDto(int? Calories, int? Fat, int? Carbs, int? Protein);