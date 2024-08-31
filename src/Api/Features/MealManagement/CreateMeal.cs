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

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MealManagement;

/// <summary>
/// Controller that handles requests to create a meal.
/// </summary>
public sealed class CreateMealController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for meal creation.
    /// </summary>
    /// <param name="validator">The validator for the request.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpPost("/api/manage/meals")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(CreateMealDto), StatusCodes.Status201Created, "application/json")]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, BadRequest, Created<CreateMealDto>>> CreateAsync(IValidator<CreateMealRequest> validator, CreateMealHandler handler, CreateMealRequest request, CancellationToken cancellationToken)
    {
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var createMealDto = await handler.HandleAsync(request, cancellationToken);

        var location = Url.Action(nameof(CreateAsync), new { id = createMealDto.Id }) ?? $"/{createMealDto.Id}";
        return TypedResults.Created(location, createMealDto);
    }
}

/// <summary>
/// The request for meal creation.
/// </summary>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">An url to the image of the meal.</param>
/// <param name="TagIds">A collection of tag ids to be included in the meal.</param>
/// <param name="RecipeIds">A collection of recipe ids to be included in the meal.</param>
public sealed record CreateMealRequest(string Title, string? Image, ICollection<int> TagIds, ICollection<int> RecipeIds);

internal sealed class CreateMealRequestValidator : AbstractValidator<CreateMealRequest>
{
    public CreateMealRequestValidator()
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
/// The handler that handles a meal creation request.
/// </summary>
public sealed class CreateMealHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Creates a <see cref="CreateMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public CreateMealHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to create a meal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="CreateMealDto"/>.
    /// </returns>
    /// <exception cref="CreateMealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    public async Task<CreateMealDto> HandleAsync(CreateMealRequest request, CancellationToken cancellationToken)
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
            throw new CreateMealValidationException(errors);
        }

        var recipesToAdd = userRecipes.Where(r => request.RecipeIds.Contains(r.Id));
        var tagsToAdd = tags.Where(t => request.TagIds.Contains(t.Id));

        var meal = new Meal
        {
            Title = request.Title,
            Image = request.Image,
            ApplicationUserId = _userContext.UserId
        };

        foreach (var recipe in recipesToAdd)
        {
            meal.Recipes.Add(recipe);
        }

        foreach (var tag in tagsToAdd)
        {
            meal.Tags.Add(tag);
        }

        _dbContext.Meals.Add(meal);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(meal);
    }

    private static Error[] ValidateIds(CreateMealRequest request, IEnumerable<int> validTagIds, IEnumerable<int> validRecipeIds, int currentUserId)
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

    private static CreateMealDto ToDto(Meal meal) =>
        new(meal.Id, meal.Title, meal.Image);
}

/// <summary>
/// The DTO to return to the client after meal creation.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="Image">An url to the image of the meal.</param>
public sealed record CreateMealDto(int Id, string Title, string? Image);