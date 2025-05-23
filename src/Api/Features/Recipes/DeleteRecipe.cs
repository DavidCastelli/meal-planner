using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Extensions;
using Api.Common.Interfaces;
using Api.Domain.Recipes;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Recipes;

/// <summary>
/// Controller that handles requests to delete a recipe.
/// </summary>
public sealed class DeleteRecipeController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for deleting a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe to be deleted.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4, TResult5}"/> object.
    /// </returns>
    [HttpDelete("/api/manage/recipes/{id:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Conflict, ForbidHttpResult, Ok>> DeleteAsync(int id, DeleteRecipeHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The handler that handles a recipe deletion request.
/// </summary>
public sealed class DeleteRecipeHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="DeleteRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public DeleteRecipeHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to delete a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="LastMealRecipeException">Is thrown if the recipe is the last recipe belonging to a meal.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipe
            .Where(r => r.Id == id)
            .Select(r => new Recipe
            {
                Id = r.Id,
                Title = r.Title,
                RecipeDetails = r.RecipeDetails,
                RecipeNutrition = r.RecipeNutrition,
                Image = r.Image,
                ApplicationUserId = r.ApplicationUserId,
            })
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            var mealGroup = await _dbContext.MealRecipe
                .GroupBy(mr => mr.MealId)
                .Select(g => new { recipeId = g.First().RecipeId, recipeCount = g.Count() })
                .Where(g => g.recipeCount == 1 && g.recipeId == id)
                .ToListAsync(cancellationToken);

            var isNotDeletable = mealGroup.Count > 0;

            if (isNotDeletable)
            {
                throw new LastMealRecipeException();
            }

            _dbContext.Recipe.Remove(recipe);

            if (recipe.Image != null)
            {
                await _dbContext.SaveChangesDeleteImageAsync(recipe.Image.ImagePath, cancellationToken);
                return;
            }

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
}