using System.Net.Mime;
using System.Text.Json;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Common.Utilities;
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
    /// <param name="data">The data for the request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpPost("/api/manage/meals")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(CreateMealDto), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, BadRequest<ValidationProblemDetails>, Created<CreateMealDto>>> CreateAsync(IValidator<CreateMealRequest> validator, CreateMealHandler handler, IFormFile data, IFormFile? image, CancellationToken cancellationToken)
    {
        var request = await JsonSerializer.DeserializeAsync<CreateMealRequest>(
            data.OpenReadStream(), cancellationToken: cancellationToken);

        if (request == null)
        {
            ModelState.AddModelError("Request.InvalidData", "The request data cannot be null.");
            var validationProblemDetails = new ValidationProblemDetails(ModelState);
            return TypedResults.BadRequest(validationProblemDetails);
        }

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var createMealDto = await handler.HandleAsync(request, image, cancellationToken);

        var location = Url.Action(nameof(CreateAsync), new { id = createMealDto.Id }) ?? $"/{createMealDto.Id}";
        return TypedResults.Created(location, createMealDto);
    }
}

/// <summary>
/// The request for meal creation.
/// </summary>
/// <param name="Title">The title of the meal.</param>
/// <param name="TagIds">A collection of tag ids to be included in the meal.</param>
/// <param name="RecipeIds">A collection of recipe ids to be included in the meal.</param>
public sealed record CreateMealRequest(string Title, ICollection<int> TagIds, ICollection<int> RecipeIds);

internal sealed class CreateMealRequestValidator : AbstractValidator<CreateMealRequest>
{
    public CreateMealRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(20);

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
    private readonly IImageProcessingInfo _imageProcessingInfo;

    /// <summary>
    /// Creates a <see cref="CreateMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="imageProcessingInfo">The image processing configuration of the application.</param>
    public CreateMealHandler(MealPlannerContext dbContext, IUserContext userContext, IImageProcessingInfo imageProcessingInfo)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _imageProcessingInfo = imageProcessingInfo;
    }

    /// <summary>
    /// Handles the database operations necessary to create a meal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="CreateMealDto"/>.
    /// </returns>
    /// <exception cref="CreateMealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    public async Task<CreateMealDto> HandleAsync(CreateMealRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var tags = await _dbContext.Tags
            .ToListAsync(cancellationToken);

        var userRecipes = await _dbContext.Recipes
            .Where(r => r.ApplicationUserId == _userContext.UserId)
            .ToListAsync(cancellationToken);

        var validRecipeIds = userRecipes.Select(r => r.Id);
        var validTagIds = tags.Select(t => t.Id);

        var validationErrors = ValidateIds(request, validTagIds, validRecipeIds, _userContext.UserId);

        if (validationErrors.Length != 0)
        {
            throw new CreateMealValidationException(validationErrors);
        }

        var recipesToAdd = userRecipes.Where(r => request.RecipeIds.Contains(r.Id));
        var tagsToAdd = tags.Where(t => request.TagIds.Contains(t.Id));

        var meal = new Meal
        {
            Title = request.Title,
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

        bool isCancellable = true;
        if (image != null)
        {
            var randomFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(_imageProcessingInfo.TempImageStoragePath, randomFileName);
            var filePath = Path.Combine(_imageProcessingInfo.ImageStoragePath, randomFileName);
            meal.ImagePath = filePath;

            var imageProcessingErrors = await FileHelpers.ProcessFormFileAsync(image, tempFilePath, filePath,
                _imageProcessingInfo.PermittedExtensions, _imageProcessingInfo.ImageSizeLimit);

            if (imageProcessingErrors.Length != 0)
            {
                throw new ImageProcessingException(imageProcessingErrors);
            }

            isCancellable = false;
        }

        _dbContext.Meals.Add(meal);
        await _dbContext.SaveChangesAsync(isCancellable ? cancellationToken : CancellationToken.None);

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
        new(meal.Id, meal.Title);
}

/// <summary>
/// The DTO to return to the client after meal creation.
/// </summary>
/// <param name="Id">The id of the meal.</param>
/// <param name="Title">The title of the meal.</param>
public sealed record CreateMealDto(int Id, string Title);