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
using Api.Domain.Recipes;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

namespace Api.Features.Recipes;

/// <summary>
/// Controller that handles requests to update a recipe.
/// </summary>
public sealed class UpdateRecipeController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for updating a recipe.
    /// </summary>
    /// <param name="id">The id of the recipe to update.</param>
    /// <param name="validator">The validator for the request.</param>
    /// <param name="handler">The handler for the request.</param>
    /// <param name="data">The data for the request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="jsonOptions">The json options used to deserialize the request data.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TReuslt4, TResult5}"/> object.
    /// </returns>
    [HttpPut("/api/manage/recipes/{id:int}")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, BadRequest<ValidationProblemDetails>, ValidationProblem, NotFound, Ok>> UpdateAsync(
        int id, IValidator<UpdateRecipeRequest> validator, UpdateRecipeHandler handler, IFormFile data, IFormFile? image,
        IOptions<JsonOptions> jsonOptions, CancellationToken cancellationToken)
    {
        UpdateRecipeRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<UpdateRecipeRequest>(
                data.OpenReadStream(), jsonOptions.Value.JsonSerializerOptions, cancellationToken: cancellationToken);
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
/// The request for updating a recipe.
/// </summary>
/// <param name="Id">The id of the recipe to update.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Details">The details of the recipe.</param>
/// <param name="Nutrition">The nutrition of the recipe.</param>
/// <param name="Directions">A list of directions belonging to the recipe.</param>
/// <param name="Tips">A list of tips belonging to the recipe.</param>
/// <param name="SubIngredients">A list of sub ingredients belonging to the recipe.</param>
public sealed record UpdateRecipeRequest(int Id, string Title, string? Description, UpdateRecipeRequestDetails Details, UpdateRecipeRequestNutrition Nutrition, IList<UpdateRecipeRequestDirection> Directions, ICollection<UpdateRecipeRequestTip> Tips, ICollection<UpdateRecipeRequestSubIngredient> SubIngredients);

/// <summary>
/// The DTO for the recipe details in an update recipe request.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record UpdateRecipeRequestDetails(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition in an update recipe request.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record UpdateRecipeRequestNutrition(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction in an update recipe request.
/// </summary>
/// <param name="Id">The direction id.</param>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record UpdateRecipeRequestDirection(int Id, int Number, string Description);

/// <summary>
/// The DTO for a tip in an update recipe request.
/// </summary>
/// <param name="Id">The tip id.</param>
/// <param name="Description">The tip description.</param>
public sealed record UpdateRecipeRequestTip(int Id, string Description);

/// <summary>
/// The DTO for a sub ingredient in an update recipe request.
/// </summary>
/// <param name="Id">The sub ingredient id.</param>
/// <param name="Name">The sub ingredient name.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record UpdateRecipeRequestSubIngredient(int Id, string? Name, ICollection<UpdateRecipeRequestIngredient> Ingredients);

/// <summary>
/// The DTO for an ingredient in an update recipe request.
/// </summary>
/// <param name="Id">The ingredient id.</param>
/// <param name="Name">The ingredient name.</param>
/// <param name="Measurement">The ingredient measurement.</param>
public sealed record UpdateRecipeRequestIngredient(int Id, string Name, string Measurement);

internal sealed class UpdateRecipeRequestRecipeDetailsValidator : AbstractValidator<UpdateRecipeRequestDetails>
{
    public UpdateRecipeRequestRecipeDetailsValidator()
    {
        RuleFor(rd => rd.PrepTime)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rd => rd.PrepTime != null);

        RuleFor(rd => rd.CookTime)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rd => rd.CookTime != null);

        RuleFor(rd => rd.Servings)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rd => rd.Servings != null);
    }
}

internal sealed class UpdateRecipeRequestRecipeNutritionValidator : AbstractValidator<UpdateRecipeRequestNutrition>
{
    public UpdateRecipeRequestRecipeNutritionValidator()
    {
        RuleFor(rn => rn.Calories)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rn => rn.Calories != null);

        RuleFor(rn => rn.Fat)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rn => rn.Fat != null);

        RuleFor(rn => rn.Carbs)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rn => rn.Carbs != null);

        RuleFor(rn => rn.Protein)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(int.MaxValue)
            .When(rn => rn.Protein != null);
    }
}

internal sealed class UpdateRecipeRequestSubIngredientValidator : AbstractValidator<UpdateRecipeRequestSubIngredient>
{
    public UpdateRecipeRequestSubIngredientValidator()
    {
        RuleFor(si => si.Name)
            .MaximumLength(20)
            .When(si => si.Name != null);

        RuleFor(si => si.Ingredients)
            .NotEmpty();

        RuleForEach(si => si.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(i => i.Name)
                .NotEmpty()
                .MaximumLength(20);

            ingredient.RuleFor(i => i.Measurement)
                .NotEmpty()
                .MaximumLength(20);
        });
    }
}

internal sealed class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThanOrEqualTo(int.MinValue)
            .LessThanOrEqualTo(int.MaxValue);

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.Description)
            .MaximumLength(255)
            .When(request => request.Description != null);

        RuleFor(request => request.Details)
            .NotNull()
            .SetValidator(new UpdateRecipeRequestRecipeDetailsValidator());

        RuleFor(request => request.Nutrition)
            .NotNull()
            .SetValidator(new UpdateRecipeRequestRecipeNutritionValidator());

        RuleFor(request => request.Directions)
            .NotEmpty();

        RuleForEach(request => request.Directions).ChildRules(direction =>
        {
            direction.RuleFor(d => d.Number)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(6);

            direction.RuleFor(d => d.Description)
                .NotEmpty()
                .MaximumLength(255);
        });

        RuleFor(request => request.Tips)
            .NotNull();

        RuleForEach(request => request.Tips).ChildRules(tip =>
        {
            tip.RuleFor(t => t.Description)
                .NotEmpty()
                .MaximumLength(255);
        });

        RuleFor(request => request.SubIngredients)
            .NotEmpty();

        RuleForEach(request => request.SubIngredients)
            .SetValidator(new UpdateRecipeRequestSubIngredientValidator());
    }
}

/// <summary>
/// The handler that handles a recipe update request.
/// </summary>
public sealed class UpdateRecipeHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IUrlGenerator _urlGenerator;
    private readonly IAuthorizationService _authorizationService;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    /// <summary>
    /// Creates a <see cref="UpdateRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    /// <param name="authorizationService">The authorization service.</param>
    /// <param name="imageProcessingOptions">The image processing configuration of the application.</param>
    public UpdateRecipeHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator,
        IAuthorizationService authorizationService, IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
        _authorizationService = authorizationService;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    /// <summary>
    /// Handles the database operations necessary to update a recipe.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="RecipeValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public async Task HandleAsync(UpdateRecipeRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var validationErrors = ValidateRecipe(request);

        if (validationErrors.Length != 0)
        {
            throw new RecipeValidationException(validationErrors);
        }

        var newRecipe = FromDto(request, _userContext.UserId);

        var oldRecipe = await _dbContext.Recipe
            .Include(r => r.Image)
            .AsSplitQuery()
            .SingleOrDefaultAsync(r => r.Id == newRecipe.Id, cancellationToken);

        if (oldRecipe == null)
        {
            throw new RecipeNotFoundException(newRecipe.Id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, oldRecipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            _dbContext.Entry(oldRecipe).CurrentValues.SetValues(newRecipe);
            _dbContext.Entry(oldRecipe.RecipeDetails).CurrentValues.SetValues(newRecipe.RecipeDetails);
            _dbContext.Entry(oldRecipe.RecipeNutrition).CurrentValues.SetValues(newRecipe.RecipeNutrition);

            foreach (var newDirection in newRecipe.Directions)
            {
                if (newDirection.Id == 0)
                {
                    oldRecipe.Directions.Add(newDirection);
                }
                else
                {
                    var oldDirection = oldRecipe.Directions.SingleOrDefault(d => d.Id == newDirection.Id);
                    if (oldDirection == null)
                    {
                        var error = RecipeErrors.DoesNotBelongToRecipe(oldRecipe.Id, newDirection.Id, nameof(Direction));
                        throw new RecipeValidationException([error]);
                    }
                    _dbContext.Entry(oldDirection).CurrentValues.SetValues(newDirection);
                }
            }

            foreach (var oldDirection in oldRecipe.Directions)
            {
                if (newRecipe.Directions.All(d => d.Id != oldDirection.Id))
                {
                    _dbContext.Remove(oldDirection);
                }
            }

            foreach (var newTip in newRecipe.Tips)
            {
                if (newTip.Id == 0)
                {
                    oldRecipe.Tips.Add(newTip);
                }
                else
                {
                    var oldTip = oldRecipe.Tips.SingleOrDefault(t => t.Id == newTip.Id);
                    if (oldTip == null)
                    {
                        var error = RecipeErrors.DoesNotBelongToRecipe(oldRecipe.Id, newTip.Id, nameof(Tip));
                        throw new RecipeValidationException([error]);
                    }
                    _dbContext.Entry(oldTip).CurrentValues.SetValues(newTip);
                }
            }

            foreach (var oldTip in oldRecipe.Tips)
            {
                if (newRecipe.Tips.All(t => t.Id != oldTip.Id))
                {
                    _dbContext.Remove(oldTip);
                }
            }

            foreach (var newSubIngredient in newRecipe.SubIngredients)
            {
                if (newSubIngredient.Id == 0)
                {
                    oldRecipe.SubIngredients.Add(newSubIngredient);
                }
                else
                {
                    var oldSubIngredient = oldRecipe.SubIngredients.SingleOrDefault(si => si.Id == newSubIngredient.Id);
                    if (oldSubIngredient == null)
                    {
                        var error = RecipeErrors.DoesNotBelongToRecipe(oldRecipe.Id, newSubIngredient.Id, nameof(SubIngredient));
                        throw new RecipeValidationException([error]);
                    }
                    _dbContext.Entry(oldSubIngredient).CurrentValues.SetValues(newSubIngredient);

                    foreach (var newIngredient in newSubIngredient.Ingredients)
                    {
                        if (newIngredient.Id == 0)
                        {
                            oldSubIngredient.Ingredients.Add(newIngredient);
                        }
                        else
                        {
                            var oldIngredient = oldSubIngredient.Ingredients.SingleOrDefault(i => i.Id == newIngredient.Id);
                            if (oldIngredient == null)
                            {
                                var error = RecipeErrors.DoesNotBelongToRecipe(oldRecipe.Id, newIngredient.Id, nameof(Ingredient));
                                throw new RecipeValidationException([error]);
                            }
                            _dbContext.Entry(oldIngredient).CurrentValues.SetValues(newIngredient);
                        }
                    }

                    foreach (var oldIngredient in oldSubIngredient.Ingredients)
                    {
                        if (newSubIngredient.Ingredients.All(i => i.Id != oldIngredient.Id))
                        {
                            _dbContext.Remove(oldIngredient);
                        }
                    }
                }
            }

            foreach (var oldSubIngredient in oldRecipe.SubIngredients)
            {
                if (newRecipe.SubIngredients.All(i => i.Id != oldSubIngredient.Id))
                {
                    _dbContext.Remove(oldSubIngredient);
                }
            }

            if (image != null)
            {
                if (oldRecipe.Image != null)
                {
                    var randomFileName = Path.GetRandomFileName();
                    var tempFilePath = Path.Combine(_imageProcessingOptions.TempImageStoragePath, randomFileName);
                    var filePath = Path.Combine(_imageProcessingOptions.ImageStoragePath, randomFileName);
                    var originalFilePath = oldRecipe.Image.ImagePath;
                    oldRecipe.Image.StorageFileName = randomFileName;
                    oldRecipe.Image.DisplayFileName = WebUtility.HtmlEncode(image.FileName);
                    oldRecipe.Image.ImagePath = filePath;
                    oldRecipe.Image.ImageUrl =
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

                    var recipeImage = new Image
                    {
                        StorageFileName = randomFileName,
                        DisplayFileName = WebUtility.HtmlEncode(image.FileName),
                        ImagePath = filePath,
                        ImageUrl = _urlGenerator.GenerateUrl("GetImageByFileName",
                            new { fileName = randomFileName }),
                        ManageableEntityId = oldRecipe.Id,
                        ManageableEntity = oldRecipe
                    };
                    oldRecipe.Image = recipeImage;

                    await _dbContext.SaveChangesSaveImageAsync(image, tempFilePath, filePath,
                        _imageProcessingOptions.PermittedExtensions, _imageProcessingOptions.ImageSizeLimit,
                        cancellationToken);
                }

                return;
            }

            if (image == null)
            {
                if (oldRecipe.Image != null)
                {
                    var imagePath = oldRecipe.Image.ImagePath;
                    oldRecipe.Image = null;
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

    private static Error[] ValidateRecipe(UpdateRecipeRequest request)
    {
        List<Error> errors = [];

        if (request.Directions.Count > RecipeErrors.MaxDirectionsCount)
        {
            errors.Add(RecipeErrors.MaxDirections());
        }

        if (request.Tips.Count > RecipeErrors.MaxTipsCount)
        {
            errors.Add(RecipeErrors.MaxTips());
        }

        if (request.SubIngredients.Count > RecipeErrors.MaxSubIngredientsCount)
        {
            errors.Add(RecipeErrors.MaxSubIngredients());
        }

        var maxIngredients = request.SubIngredients.Any(si => si.Ingredients.Count > RecipeErrors.MaxIngredientsCount);
        if (maxIngredients)
        {
            errors.Add(RecipeErrors.MaxIngredients());
        }

        var directions = request.Directions.OrderBy(d => d.Number).ToList();

        if (directions[0].Number != 1)
        {
            errors.Add(RecipeErrors.DirectionsStartAtOne());
        }

        var isSequential = true;
        for (int i = 1; i < directions.Count; i++)
        {
            if (directions[i].Number != directions[i - 1].Number + 1)
            {
                isSequential = false;
            }
        }

        if (!isSequential)
        {
            errors.Add(RecipeErrors.DirectionsNotSequential());
        }

        var subIngredients = request.SubIngredients;

        if (subIngredients.Count == 1 && subIngredients.First().Name != null)
        {
            errors.Add(RecipeErrors.SingleSubIngredientName());
        }

        if (subIngredients.Count > 1 && subIngredients.Any(si => si.Name == null))
        {
            errors.Add(RecipeErrors.MultipleSubIngredientName());
        }

        return [.. errors];
    }

    private static Recipe FromDto(UpdateRecipeRequest request, int userId)
    {
        var recipeDetails = new RecipeDetails()
        {
            PrepTime = request.Details.PrepTime,
            CookTime = request.Details.CookTime,
            Servings = request.Details.Servings
        };

        var recipeNutrition = new RecipeNutrition()
        {
            Calories = request.Nutrition.Calories,
            Fat = request.Nutrition.Fat,
            Carbs = request.Nutrition.Carbs,
            Protein = request.Nutrition.Protein
        };

        var recipe = new Recipe()
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            RecipeDetails = recipeDetails,
            RecipeNutrition = recipeNutrition,
            ApplicationUserId = userId
        };

        foreach (var directionDto in request.Directions)
        {
            var direction = new Direction { Id = directionDto.Id, Number = directionDto.Number, Description = directionDto.Description };

            recipe.Directions.Add(direction);
        }

        foreach (var tipDto in request.Tips)
        {
            var tip = new Tip { Id = tipDto.Id, Description = tipDto.Description };

            recipe.Tips.Add(tip);
        }

        foreach (var subIngredientDto in request.SubIngredients)
        {
            var subIngredient = new SubIngredient { Id = subIngredientDto.Id, Name = subIngredientDto.Name };

            foreach (var ingredientDto in subIngredientDto.Ingredients)
            {
                var ingredient = new Ingredient { Id = ingredientDto.Id, Name = ingredientDto.Name, Measurement = ingredientDto.Measurement };

                subIngredient.Ingredients.Add(ingredient);
            }

            recipe.SubIngredients.Add(subIngredient);
        }

        return recipe;
    }
}