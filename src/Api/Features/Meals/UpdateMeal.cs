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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

namespace Api.Features.Meals;

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
    /// <param name="data">The data for the request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="jsonOptions">The json options used to deserialize the request data.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4, TResult5}"/> object.
    /// </returns>
    [HttpPut("/api/manage/meals/{id:int}")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Meals")]
    public async Task<Results<UnauthorizedHttpResult, BadRequest<ValidationProblemDetails>, ValidationProblem, NotFound, Ok>> UpdateAsync(
        int id, IValidator<UpdateMealRequest> validator, IFormFile data, IFormFile? image, UpdateMealHandler handler,
        IOptions<JsonOptions> jsonOptions, CancellationToken cancellationToken)
    {
        UpdateMealRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<UpdateMealRequest>(
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

        if (id != request.Id)
        {
            ModelState.AddModelError("Request.InvalidId", "The request id must match the route id.");
            var validationProblemDetails = new ValidationProblemDetails(ModelState);
            return TypedResults.BadRequest(validationProblemDetails);
        }

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        await handler.HandleAsync(request, image, cancellationToken);

        return TypedResults.Ok();
    }
}

/// <summary>
/// The request to update a meal.
/// </summary>
/// <param name="Id">The id of the meal to update.</param>
/// <param name="Title">The title of the meal.</param>
/// <param name="TagIds">A collection of tag ids to be included in the meal.</param>
/// <param name="RecipeIds">A collection of recipe ids to be included in the meal.</param>
public sealed record UpdateMealRequest(int Id, string Title, ICollection<int> TagIds, ICollection<int> RecipeIds);

internal sealed class UpdateMealRequestValidator : AbstractValidator<UpdateMealRequest>
{
    public UpdateMealRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThanOrEqualTo(int.MinValue)
            .LessThanOrEqualTo(int.MaxValue);

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
/// The handler that handles a meal update request.
/// </summary>
public sealed class UpdateMealHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IUrlGenerator _urlGenerator;
    private readonly IAuthorizationService _authorizationService;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    /// <summary>
    /// Creates a <see cref="UpdateMealHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    /// <param name="authorizationService">The authorization service.</param>
    /// <param name="imageProcessingOptions">The image processing configuration of the application.</param>
    public UpdateMealHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator,
        IAuthorizationService authorizationService, IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
        _authorizationService = authorizationService;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    /// <summary>
    /// Handles the database operations necessary to update a meal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="MealNotFoundException">Is thrown if the meal could not be found in the data store.</exception>
    /// <exception cref="MealValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public async Task HandleAsync(UpdateMealRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var newMeal = FromDto(request, _userContext.UserId);

        var oldMeal = await _dbContext.Meal
            .Where(m => m.Id == newMeal.Id)
            .Select(m => new Meal(m.Tags, m.Recipes.Select(r => new Recipe
            {
                Id = r.Id,
                Title = r.Title,
                RecipeDetails = r.RecipeDetails,
                RecipeNutrition = r.RecipeNutrition,
                ApplicationUserId = r.ApplicationUserId
            }).ToList())
            {
                Id = m.Id,
                Title = m.Title,
                Image = m.Image,
                ApplicationUserId = m.ApplicationUserId
            })
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (oldMeal == null)
        {
            throw new MealNotFoundException(newMeal.Id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, oldMeal, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
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

            var validationErrors = ValidateMeal(request, validTagIds, validRecipeIds, _userContext.UserId);

            if (validationErrors.Length != 0)
            {
                throw new MealValidationException(validationErrors);
            }

            var newTags = tags
                .Where(t => request.TagIds.Contains(t.Id))
                .ToList();
            var newRecipes = userRecipes
                .Where(r => request.RecipeIds.Contains(r.Id))
                .ToList();

            var tagsToAdd = newTags
                .ExceptBy(oldMeal.Tags.Select(t => t.Id), t => t.Id)
                .ToList();
            var recipesToAdd = newRecipes
                .ExceptBy(oldMeal.Recipes.Select(r => r.Id), r => r.Id)
                .ToList();

            _dbContext.Attach(oldMeal);
            _dbContext.AttachRange(tagsToAdd);
            _dbContext.AttachRange(recipesToAdd);

            _dbContext.Entry(oldMeal).CurrentValues.SetValues(newMeal);

            foreach (var tagToAdd in tagsToAdd)
            {
                oldMeal.Tags.Add(tagToAdd);
            }

            foreach (var oldTag in oldMeal.Tags.ToList())
            {
                if (newTags.All(t => t.Id != oldTag.Id))
                {
                    oldMeal.Tags.Remove(oldTag);
                }
            }

            foreach (var recipeToAdd in recipesToAdd)
            {
                oldMeal.Recipes.Add(recipeToAdd);
            }

            foreach (var oldRecipe in oldMeal.Recipes.ToList())
            {
                if (newRecipes.All(r => r.Id != oldRecipe.Id))
                {
                    oldMeal.Recipes.Remove(oldRecipe);
                }
            }

            if (image != null)
            {
                if (oldMeal.Image != null)
                {
                    var randomFileName = Path.GetRandomFileName();
                    var tempFilePath = Path.Combine(_imageProcessingOptions.TempImageStoragePath, randomFileName);
                    var filePath = Path.Combine(_imageProcessingOptions.ImageStoragePath, randomFileName);
                    var originalFilePath = oldMeal.Image.ImagePath;
                    oldMeal.Image.StorageFileName = randomFileName;
                    oldMeal.Image.DisplayFileName = WebUtility.HtmlEncode(image.FileName);
                    oldMeal.Image.ImagePath = filePath;
                    oldMeal.Image.ImageUrl =
                        _urlGenerator.GenerateUrl("GetImageByFileName", new { fileName = randomFileName });

                    await _dbContext.SaveChangesSaveImageAsync(image, tempFilePath, filePath,
                        _imageProcessingOptions.PermittedExtensions, _imageProcessingOptions.ImageSizeLimit,
                        cancellationToken, originalFilePath);
                }
                else
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
                        ManageableEntityId = oldMeal.Id,
                        ManageableEntity = oldMeal
                    };
                    oldMeal.Image = mealImage;

                    await _dbContext.SaveChangesSaveImageAsync(image, tempFilePath, filePath,
                        _imageProcessingOptions.PermittedExtensions, _imageProcessingOptions.ImageSizeLimit,
                        cancellationToken);
                }

                return;
            }

            if (image == null)
            {
                if (oldMeal.Image != null)
                {
                    var imagePath = oldMeal.Image.ImagePath;
                    oldMeal.Image = null;
                    await _dbContext.SaveChangesDeleteImageAsync(imagePath, cancellationToken);
                    return;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgresException)
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

    private static Error[] ValidateMeal(UpdateMealRequest request, IEnumerable<int> validTagIds, IEnumerable<int> validRecipeIds, int currentUserId)
    {
        List<Error> errors = [];

        if (request.TagIds.Count > MealErrors.MaxTagsCount)
        {
            errors.Add(MealErrors.MaxTags());
        }

        if (request.RecipeIds.Count > MealErrors.MaxRecipesCount)
        {
            errors.Add(MealErrors.MaxRecipes());
        }

        if (request.RecipeIds.Distinct().Count() != request.RecipeIds.Count)
        {
            errors.Add(MealErrors.DuplicateRecipeId());
        }

        if (request.TagIds.Distinct().Count() != request.TagIds.Count)
        {
            errors.Add(MealErrors.DuplicateTagId());
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

    private static Meal FromDto(UpdateMealRequest request, int userId) => new() { Id = request.Id, Title = request.Title, ApplicationUserId = userId };
}