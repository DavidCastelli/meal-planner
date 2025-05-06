using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Extensions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Domain.ManageableEntities;
using Api.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Meals;

/// <summary>
/// Controller that handles requests to delete a meal.
/// </summary>
public sealed class DeleteMealController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for meal deletion.
    /// </summary>
    /// <param name="id">The id of the meal to be deleted.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3}"/> object.
    /// </returns>
    [HttpDelete("/api/manage/meals/{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok>> DeleteAsync(int id, DeleteMealHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The handler that handles a meal deletion request.
/// </summary>
public sealed class DeleteMealHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="DeleteMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public DeleteMealHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to delete a meal.
    /// </summary>
    /// <param name="id">The id of the meal to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var meal = await _dbContext.ManageableEntity
            .Where(m => m.Id == id)
            .Select(m => new ManageableEntity
            {
                Id = m.Id,
                Title = m.Title,
                Image = m.Image,
                ApplicationUserId = m.ApplicationUserId
            })
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            if (meal.Image != null)
            {
                await _dbContext.SaveChangesDeleteImageAsync(meal.Image.ImagePath, cancellationToken);
                return;
            }

            _dbContext.ManageableEntity.Remove(meal);
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