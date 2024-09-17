using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MealManagement;

/// <summary>
/// Controller that handles requests to get the image belonging to a meal.
/// </summary>
public sealed class GetMealImageByIdController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for getting the image of a meal.
    /// </summary>
    /// <param name="id">The id of the meal whose image to get.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpGet("/api/manage/meals/{id:int}/image")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PhysicalFileHttpResult), StatusCodes.Status200OK, MediaTypeNames.Image.Jpeg)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, PhysicalFileHttpResult>> GetByIdAsync(int id, GetMealImageByIdHandler handler, CancellationToken cancellationToken)
    {
        var imagePath = await handler.HandleAsync(id, cancellationToken);
        return TypedResults.PhysicalFile(imagePath, MediaTypeNames.Image.Jpeg);
    }
}

/// <summary>
/// The handler that handles getting a meals image path.
/// </summary>
public sealed class GetMealImageByIdHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="GetMealImageByIdHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public GetMealImageByIdHandler(MealPlannerContext dbContext, IUserContext userContext,
        IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operation necessary to get the image path of a meal by id.
    /// </summary>
    /// <param name="id">The id of the meal whose image path to get.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous read operation.
    /// The result of the task upon completion returns the image path of the meal.
    /// </returns>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="MealImageNotFoundException">Is thrown if the meals image path is null.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<string> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var meal = await _dbContext.Meals
            .AsNoTracking()
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(id);
        }

        if (meal.ImagePath == null)
        {
            throw new MealImageNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            return meal.ImagePath;
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