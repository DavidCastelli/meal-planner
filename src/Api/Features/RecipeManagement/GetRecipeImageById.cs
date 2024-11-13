using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.RecipeManagement;

/// <summary>
/// Controller that handles requests to get the image belonging to a recipe.
/// </summary>
public sealed class GetRecipeImageByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting the image of a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe whose image to get.</param>
    /// <param name="handler">The handler for the request</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpGet("/api/manage/recipes/{id:int}/image", Name = "GetRecipeImageById")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PhysicalFileHttpResult), StatusCodes.Status200OK, MediaTypeNames.Image.Jpeg)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, PhysicalFileHttpResult>> GetByIdAsync(int id, GetRecipeImageByIdHandler handler, CancellationToken cancellationToken)
    {
        var imagePath = await handler.HandleAsync(id, cancellationToken);
        return TypedResults.PhysicalFile(imagePath, MediaTypeNames.Image.Jpeg);
    }
}

/// <summary>
/// The handler that handles getting a recipes image path.
/// </summary>
public sealed class GetRecipeImageByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetRecipeImageByIdHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetRecipeImageByIdHandler(MealPlannerContext dbContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operation necessary to get the image path of a recipe by id.
    /// </summary>
    /// <param name="id">The id of the recipe whose image path to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns the image path of the recipe.
    /// </returns>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="RecipeImageNotFoundException">Is thrown if the recipes image path is null.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<string> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipes
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(id);
        }

        if (recipe.ImagePath == null)
        {
            throw new RecipeImageNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return recipe.ImagePath;
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