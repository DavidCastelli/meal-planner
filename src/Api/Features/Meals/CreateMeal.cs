using System.Net;
using System.Net.Mime;
using System.Text.Json;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Extensions;
using Api.Common.Interfaces;
using Api.Common.Options;
using Api.Common.Utilities;
using Api.Domain.Images;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

namespace Api.Features.Meals;

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
    /// <param name="jsonOptions">The json options used to deserialize the request data.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpPost("/api/manage/meals")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, BadRequest<ValidationProblemDetails>, Created>> CreateAsync(
        IValidator<CreateMealRequest> validator, CreateMealHandler handler, IFormFile data, IFormFile? image,
        IOptions<JsonOptions> jsonOptions, CancellationToken cancellationToken)
    {
        CreateMealRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<CreateMealRequest>(
                data.OpenReadStream(), jsonOptions.Value.JsonSerializerOptions, cancellationToken);
        }
        catch (JsonException)
        {
            ModelState.AddModelError("Request.InvalidJson", "Failed to deserialize the request, the request JSON is invalid.");
            var validationProblemDetails = new ValidationProblemDetails(ModelState);
            return TypedResults.BadRequest(validationProblemDetails);
        }

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

        var mealId = await handler.HandleAsync(request, image, cancellationToken);

        var location = Url.Link("GetMealById", new { id = mealId });
        return TypedResults.Created(location);
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

        RuleForEach(request => request.TagIds)
            .GreaterThanOrEqualTo(int.MinValue)
            .LessThanOrEqualTo(int.MaxValue);

        RuleFor(request => request.RecipeIds)
            .NotEmpty();

        RuleForEach(request => request.RecipeIds)
            .GreaterThanOrEqualTo(int.MinValue)
            .LessThanOrEqualTo(int.MaxValue);
    }
}

/// <summary>
/// The handler that handles a meal creation request.
/// </summary>
public sealed class CreateMealHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IUrlGenerator _urlGenerator;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    /// <summary>
    /// Creates a <see cref="CreateMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    /// <param name="imageProcessingOptions">The image processing configuration of the application.</param>
    public CreateMealHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator,
        IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    /// <summary>
    /// Handles the database operations necessary to create a meal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns an integer specifying the id of the created meal.
    /// </returns>
    /// <exception cref="MealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public async Task<int> HandleAsync(CreateMealRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var mealCount = await _dbContext.Meal
            .CountAsync(m => m.ApplicationUserId == _userContext.UserId, cancellationToken);

        var tags = await _dbContext.Tag
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var userRecipes = await _dbContext.Recipe
            .Where(r => r.ApplicationUserId == _userContext.UserId)
            .Select(r => new Recipe
            {
                Id = r.Id,
                Title = r.Title,
                RecipeDetails = r.RecipeDetails,
                RecipeNutrition = r.RecipeNutrition,
                ApplicationUserId = r.ApplicationUserId
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var validTagIds = tags.Select(t => t.Id);
        var validRecipeIds = userRecipes.Select(r => r.Id);

        var validationErrors = ValidateMeal(request, mealCount, validTagIds, validRecipeIds, _userContext.UserId);

        if (validationErrors.Length != 0)
        {
            throw new MealValidationException(validationErrors);
        }

        var tagsToAdd = tags.Where(t => request.TagIds.Contains(t.Id)).ToList();
        var recipesToAdd = userRecipes.Where(r => request.RecipeIds.Contains(r.Id)).ToList();

        _dbContext.AttachRange(tagsToAdd);
        _dbContext.AttachRange(recipesToAdd);

        var meal = FromDto(request, _userContext.UserId);

        foreach (var recipe in recipesToAdd)
        {
            meal.Recipes.Add(recipe);
        }

        foreach (var tag in tagsToAdd)
        {
            meal.Tags.Add(tag);
        }

        if (image != null)
        {
            var randomFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(_imageProcessingOptions.TempImageStoragePath, randomFileName);
            var filePath = Path.Combine(_imageProcessingOptions.ImageStoragePath, randomFileName);

            var mealImage = new Image
            {
                StorageFileName = randomFileName,
                DisplayFileName = WebUtility.HtmlEncode(image.FileName),
                ImagePath = filePath,
                ImageUrl = _urlGenerator.GenerateUrl("GetImageByFileName",
                    new { fileName = randomFileName }),
                ManageableEntityId = meal.Id,
                ManageableEntity = meal
            };
            meal.Image = mealImage;

            _dbContext.Meal.Add(meal);
            await _dbContext.SaveChangesSaveImageAsync(image, tempFilePath, filePath,
                _imageProcessingOptions.PermittedExtensions, _imageProcessingOptions.ImageSizeLimit, cancellationToken);

            return meal.Id;
        }

        _dbContext.Meal.Add(meal);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.GetBaseException() is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                } postgresException)
            {
                var cause = postgresException.ConstraintName switch
                {
                    "IX_ManageableEntity_ApplicationUserId_Title" => "Title",
                    "IX_SubIngredient_RecipeId_Name" => "SubIngredient names",
                    "IX_Direction_RecipeId_Number" => "Direction numbers",
                    _ => null
                };

                throw new UniqueConstraintViolationException(cause);
            }

            throw;
        }

        return meal.Id;
    }

    private static Error[] ValidateMeal(CreateMealRequest request, int mealCount, IEnumerable<int> validTagIds, IEnumerable<int> validRecipeIds, int currentUserId)
    {
        List<Error> errors = [];

        if (mealCount > MealErrors.MaxMealsCount)
        {
            errors.Add(MealErrors.MaxMeals());
        }

        if (request.TagIds.Count > MealErrors.MaxTagsCount)
        {
            errors.Add(MealErrors.MaxTags());
        }

        if (request.RecipeIds.Count > MealErrors.MaxRecipesCount)
        {
            errors.Add(MealErrors.MaxRecipes());
        }

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

    private static Meal FromDto(CreateMealRequest request, int userId) => new() { Title = request.Title, ApplicationUserId = userId };
}