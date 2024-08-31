using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.Tags;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MealManagement;

/// <summary>
/// Controller that handles requests to update a meal.
/// </summary>
public sealed class UpdateMealController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for updating a meal.
    /// </summary>
    /// <param name="id">The id of the meal to update.</param>
    /// <param name="validator">The validator for the request.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4, TResult5}"/> object.
    /// </returns>
    [HttpPut("/api/manage/meals/{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UpdateMealDto), StatusCodes.Status200OK, "application/json")]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, BadRequest, ValidationProblem, NotFound, Ok<UpdateMealDto>>> UpdateAsync(int id, IValidator<UpdateMealRequest> validator,
        UpdateMealRequest request, UpdateMealHandler handler, CancellationToken cancellationToken)
    {

        if (id != request.Id)
        {
            return TypedResults.BadRequest();
        }

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var updateMealDto = await handler.HandleAsync(request, cancellationToken);

        return TypedResults.Ok(updateMealDto);
    }
}

/// <summary>
/// The request to update a meal.
/// </summary>
/// <param name="Id">The id of the meal to update.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">An url to the image of the meal.</param>
/// <param name="TagIds">A collection of tag ids to be included in the meal.</param>
/// <param name="RecipeIds">A collection of recipe ids to be included in the meal.</param>
public sealed record UpdateMealRequest(int Id, string Title, string? Image, ICollection<int> TagIds, ICollection<int> RecipeIds);

internal sealed class UpdateMealRequestValidator : AbstractValidator<UpdateMealRequest>
{
    public UpdateMealRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.Image)
            .MaximumLength(255)
            .When(request => request.Image != null);

        RuleFor(request => request.TagIds)
            .NotNull();

        RuleFor(request => request.RecipeIds)
            .NotEmpty();
    }
}

/// <summary>
/// The handler that handles a meal update request.
/// </summary>
public sealed class UpdateMealHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a <see cref="UpdateMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public UpdateMealHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the database operations necessary to update a meal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="UpdateMealDto"/>.
    /// </returns>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="UpdateMealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<UpdateMealDto> HandleAsync(UpdateMealRequest request, CancellationToken cancellationToken)
    {
        var meal = await _dbContext.Meals
            .Include(m => m.Recipes)
            .Include(m => m.Tags)
            .AsSplitQuery()
            .SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (meal == null)
        {
            throw new MealNotFoundException(request.Id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, meal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            var tags = await _dbContext.Tags
                .ToListAsync(cancellationToken);

            var userRecipes = await _dbContext.Recipes
                .Where(r => r.ApplicationUserId == _userContext.UserId)
                .ToListAsync(cancellationToken);

            var validRecipeIds = userRecipes.Select(r => r.Id);
            var validTagIds = tags.Select(t => t.Id);

            var errors = ValidateIds(request, validTagIds, validRecipeIds, _userContext.UserId);

            if (errors.Length != 0)
            {
                throw new UpdateMealValidationException(errors);
            }

            var recipesToAdd = userRecipes.Where(r => request.RecipeIds.Contains(r.Id));
            var tagsToAdd = tags.Where(t => request.TagIds.Contains(t.Id));

            meal.Title = request.Title;
            meal.Image = request.Image;
            
            meal.Tags.Clear();
            meal.Recipes.Clear();

            foreach (var tag in tagsToAdd)
            {
                meal.Tags.Add(tag);
            }

            foreach (var recipe in recipesToAdd)
            {
                meal.Recipes.Add(recipe);
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);

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

    private static Error[] ValidateIds(UpdateMealRequest request, IEnumerable<int> validTagIds, IEnumerable<int> validRecipeIds, int currentUserId)
    {
        List<Error> errors = [];

        if (!request.RecipeIds.All(validRecipeIds.Contains))
        {
            errors.Add(RecipeErrors.DoesNotBelongToUser(currentUserId));
        }

        if (!request.TagIds.All(validTagIds.Contains))
        {
            errors.Add(TagErrors.NotFound());
        }

        return [.. errors];
    }

    private static UpdateMealDto ToDto(Meal meal)
    {
        var tagDtos = new List<UpdateTagDto>();

        foreach (var tag in meal.Tags)
        {
            var tagDto = new UpdateTagDto(tag.Id, tag.Type);
            
            tagDtos.Add(tagDto);
        }
        
        var recipeDtos = new List<UpdateMealRecipeDto>();

        foreach (var recipe in meal.Recipes)
        {
            var recipeDto = new UpdateMealRecipeDto(recipe.Id, recipe.Title, recipe.Description, recipe.Image);
            
            recipeDtos.Add(recipeDto);
        }
        
        return new UpdateMealDto(meal.Id, meal.Title, meal.Image, tagDtos, recipeDtos, meal.ApplicationUserId);
    }
}

/// <summary>
/// The DTO to return to the client after updating a meal.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">An url to the image of the meal.</param>
/// <param name="Tags">A collection of tags belonging to the meal.</param>
/// <param name="Recipes">A collection of recipes belonging to the meal.</param>
/// <param name="ApplicationUserId">The id of the user who the meal belongs to.</param>
public sealed record UpdateMealDto(int Id, string Title, string? Image, ICollection<UpdateTagDto> Tags, ICollection<UpdateMealRecipeDto> Recipes, int ApplicationUserId);

/// <summary>
/// The DTO for a tag to return to the client.
/// </summary>
/// <param name="Id">The id of the tag.</param>
/// <param name="TagType">The type of tag.</param>
public sealed record UpdateTagDto(int Id, TagType TagType);

/// <summary>
/// The DTO for a recipe to return to the client.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Image">An url to the image of the recipe.</param>
public sealed record UpdateMealRecipeDto(int Id, string Title, string? Description, string? Image);