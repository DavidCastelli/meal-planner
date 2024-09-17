using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain.Ingredients;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.RecipeManagement;

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
    [HttpGet("/api/manage/recipes/{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetByIdRecipeDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<GetByIdRecipeDto>>> GetByIdAsync(int id, GetRecipeByIdHandler handler, CancellationToken cancellationToken)
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
    /// The result of the task upon completion returns a <see cref="GetByIdRecipeDto"/>.
    /// </returns>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<GetByIdRecipeDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipes
            .Include(r => r.Meals)
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return ToDto(recipe);
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

    private static GetByIdRecipeDto ToDto(Recipe recipe)
    {
        var recipeDetailsDto = new GetByIdRecipeDetailsDto(recipe.RecipeDetails.PrepTime, recipe.RecipeDetails.CookTime, recipe.RecipeDetails.Servings);
        var recipeNutritionDto = new GetByIdRecipeNutritionDto(recipe.RecipeNutrition.Calories, recipe.RecipeNutrition.Fat, recipe.RecipeNutrition.Carbs, recipe.RecipeNutrition.Protein);

        var directionDtos = new List<GetByIdDirectionDto>();

        foreach (var direction in recipe.Directions)
        {
            var directionDto = new GetByIdDirectionDto(direction.Number, direction.Description);
            directionDtos.Add(directionDto);
        }

        var tipDtos = new List<GetByIdTipDto>();

        foreach (var tip in recipe.Tips)
        {
            var tipDto = new GetByIdTipDto(tip.Description);
            tipDtos.Add(tipDto);
        }

        var mealDtos = new List<GetByIdRecipeMealDto>();

        foreach (var meal in recipe.Meals)
        {
            var mealDto = new GetByIdRecipeMealDto(meal.Id, meal.Title);
            mealDtos.Add(mealDto);
        }

        var subIngredientDtos = new List<GetByIdSubIngredientDto>();

        foreach (var subIngredient in recipe.SubIngredients)
        {
            var ingredientDtos = new List<GetByIdIngredientDto>();
            foreach (var ingredient in subIngredient.Ingredients)
            {
                var ingredientDto = new GetByIdIngredientDto(ingredient.Name, ingredient.Measurement);
                ingredientDtos.Add(ingredientDto);
            }

            var subIngredientDto = new GetByIdSubIngredientDto(subIngredient.Name, ingredientDtos);

            subIngredientDtos.Add(subIngredientDto);
        }

        return new GetByIdRecipeDto(recipe.Id, recipe.Title, recipe.Description, recipeDetailsDto, recipeNutritionDto, directionDtos, tipDtos, mealDtos, subIngredientDtos, recipe.ApplicationUserId);
    }
}

/// <summary>
/// The DTO to return to the client after getting a recipe by id.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Details">The recipe details.</param>
/// <param name="Nutrition">The recipe nutrition.</param>
/// <param name="Directions">A list of directions belonging to the recipe.</param>
/// <param name="Tips">A collection of tips belonging to the recipe.</param>
/// <param name="Meals">A collections of meals which the recipe belongs to.</param>
/// <param name="SubIngredients">A collection of sub ingredients belonging to the recipe.</param>
/// <param name="ApplicationUserId">The id of the user who the meal belongs to.</param>
public sealed record GetByIdRecipeDto(int Id, String Title, String? Description, GetByIdRecipeDetailsDto Details, GetByIdRecipeNutritionDto Nutrition, IList<GetByIdDirectionDto> Directions, ICollection<GetByIdTipDto> Tips, ICollection<GetByIdRecipeMealDto> Meals, ICollection<GetByIdSubIngredientDto> SubIngredients, int ApplicationUserId);

/// <summary>
/// The DTO for recipe details to return to the client.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record GetByIdRecipeDetailsDto(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition to return to the client.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record GetByIdRecipeNutritionDto(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction to return to the client.
/// </summary>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record GetByIdDirectionDto(int Number, string Description);

/// <summary>
/// The DTO for a tip to return to the client.
/// </summary>
/// <param name="Description">The tip description.</param>
public sealed record GetByIdTipDto(string Description);

/// <summary>
/// The Dto for a meal to return to the client.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
public sealed record GetByIdRecipeMealDto(int Id, string Title);

/// <summary>
/// The DTO for a sub ingredient to return to the client.
/// </summary>
/// <param name="Name">The name of the sub ingredient.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record GetByIdSubIngredientDto(string? Name, ICollection<GetByIdIngredientDto> Ingredients);

/// <summary>
/// The DTO for an ingredient to return to the client.
/// </summary>
/// <param name="Name">The name of the ingredient.</param>
/// <param name="Measurement">The measurement of the ingredient.</param>
public sealed record GetByIdIngredientDto(string Name, string Measurement);