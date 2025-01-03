using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Recipes;

/// <summary>
/// Controller that handles requests to get a recipe by id.
/// </summary>
public sealed class GetRecipeByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting a recipe by id.
    /// </summary>
    /// <param name="id">The id of the recipe to get.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpGet("/api/manage/recipes/{id:int}", Name = "GetRecipeById")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetRecipeByIdDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<GetRecipeByIdDto>>> GetByIdAsync(int id, GetRecipeByIdHandler handler, CancellationToken cancellationToken)
    {
        var getRecipeByIdDto = await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok(getRecipeByIdDto);
    }
}

/// <summary>
/// The handler that handles a get recipe by id request.
/// </summary>
public sealed class GetRecipeByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetRecipeByIdHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetRecipeByIdHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to get a recipe by id.
    /// </summary>
    /// <param name="id">The id of the recipe to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="GetRecipeByIdDto"/>.
    /// </returns>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<GetRecipeByIdDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipe
            .Where(r => r.Id == id)
            .Select(r => new GetRecipeByIdDto(
                r.Id,
                r.Title,
                r.Image == null ? null : new GetRecipeByIdImageDto(r.Image.Id, r.Image.StorageFileName, r.Image.DisplayFileName, r.Image.ImageUrl),
                r.Description,
                new GetRecipeByIdDetailsDto(r.RecipeDetails.PrepTime, r.RecipeDetails.CookTime, r.RecipeDetails.Servings),
                new GetRecipeByIdNutritionDto(r.RecipeNutrition.Calories, r.RecipeNutrition.Fat, r.RecipeNutrition.Carbs, r.RecipeNutrition.Protein),
                r.Directions.Select(d => new GetRecipeByIdDirectionDto(d.Id, d.Number, d.Description)).ToList(),
                r.Tips.Select(t => new GetRecipeByIdTipDto(t.Id, t.Description)).ToList(),
                r.Meals.Select(m => new GetRecipeByIdMealDto(m.Id, m.Title)).ToList(),
                r.SubIngredients.Select(si => new GetRecipeByIdSubIngredientDto(
                    si.Id,
                    si.Name,
                    si.Ingredients.Select(i => new GetRecipeByIdIngredientDto(
                        i.Id,
                        i.Name,
                        i.Measurement)).ToList())).ToList(), r.ApplicationUserId))
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(id);
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return recipe;
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
/// The DTO to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Image">The image of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Details">The recipe details.</param>
/// <param name="Nutrition">The recipe nutrition.</param>
/// <param name="Directions">A list of directions belonging to the recipe.</param>
/// <param name="Tips">A collection of tips belonging to the recipe.</param>
/// <param name="Meals">A collections of meals which the recipe belongs to.</param>
/// <param name="SubIngredients">A collection of sub ingredients belonging to the recipe.</param>
/// <param name="ApplicationUserId">The application user id of the user who the recipe belongs to.</param>
public sealed record GetRecipeByIdDto(int Id, String Title, GetRecipeByIdImageDto? Image, String? Description,
    GetRecipeByIdDetailsDto Details, GetRecipeByIdNutritionDto Nutrition, IList<GetRecipeByIdDirectionDto> Directions,
    ICollection<GetRecipeByIdTipDto> Tips, ICollection<GetRecipeByIdMealDto> Meals,
    ICollection<GetRecipeByIdSubIngredientDto> SubIngredients, int ApplicationUserId) : IAuthorizable;

/// <summary>
/// The DTO for an image to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the image.</param>
/// <param name="StorageFileName">The storage file name of the image.</param>
/// <param name="DisplayFileName">The display file name of the image.</param>
/// <param name="ImageUrl">A valid URL for the image.</param>
public sealed record GetRecipeByIdImageDto(int Id, string StorageFileName, string DisplayFileName, string ImageUrl);

/// <summary>
/// The DTO for recipe details to return to the client when getting a recipe by id.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record GetRecipeByIdDetailsDto(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record GetRecipeByIdNutritionDto(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The direction id.</param>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record GetRecipeByIdDirectionDto(int Id, int Number, string Description);

/// <summary>
/// The DTO for a tip to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The tip id.</param>
/// <param name="Description">The tip description.</param>
public sealed record GetRecipeByIdTipDto(int Id, string Description);

/// <summary>
/// The Dto for a meal to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
public sealed record GetRecipeByIdMealDto(int Id, string Title);

/// <summary>
/// The DTO for a sub ingredient to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the sub ingredient.</param>
/// <param name="Name">The name of the sub ingredient.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record GetRecipeByIdSubIngredientDto(int Id, string? Name, ICollection<GetRecipeByIdIngredientDto> Ingredients);

/// <summary>
/// The DTO for an ingredient to return to the client when getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the ingredient.</param>
/// <param name="Name">The name of the ingredient.</param>
/// <param name="Measurement">The measurement of the ingredient.</param>
public sealed record GetRecipeByIdIngredientDto(int Id, string Name, string Measurement);