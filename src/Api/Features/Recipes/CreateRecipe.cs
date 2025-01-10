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

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Npgsql;

namespace Api.Features.Recipes;

/// <summary>
/// Controller that handles requests to create a recipe.
/// </summary>
public sealed class CreateRecipeController : ApiControllerBase
{
    /// <summary>
    /// Action method responsible for recipe creation.
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
    [HttpPost("/api/manage/recipes")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, BadRequest<ValidationProblemDetails>, Created>> CreateAsync(
        IValidator<CreateRecipeRequest> validator, CreateRecipeHandler handler, IFormFile data, IFormFile? image,
        IOptions<JsonOptions> jsonOptions, CancellationToken cancellationToken)
    {
        CreateRecipeRequest? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<CreateRecipeRequest>(
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

        var recipeId = await handler.HandleAsync(request, image, cancellationToken);

        var location = Url.Link("GetRecipeById", new { id = recipeId });
        return TypedResults.Created(location);
    }
}

/// <summary>
/// The request for recipe creation.
/// </summary>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Details">The details of the recipe.</param>
/// <param name="Nutrition">The nutrition of the recipe.</param>
/// <param name="Directions">A list of directions belonging to the recipe.</param>
/// <param name="Tips">A list of tips belonging to the recipe.</param>
/// <param name="SubIngredients">A list of sub ingredients belonging to the recipe.</param>
public sealed record CreateRecipeRequest(string Title, string? Description, CreateRecipeRequestDetails Details, CreateRecipeRequestNutrition Nutrition, IList<CreateRecipeRequestDirection> Directions, ICollection<CreateRecipeRequestTip> Tips, ICollection<CreateRecipeRequestSubIngredient> SubIngredients);

/// <summary>
/// The DTO for the recipe details in a create recipe request.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record CreateRecipeRequestDetails(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition in a create recipe request.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record CreateRecipeRequestNutrition(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction in a create recipe request.
/// </summary>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record CreateRecipeRequestDirection(int Number, string Description);

/// <summary>
/// The DTO for a tip in a create recipe request.
/// </summary>
/// <param name="Description">The tip description.</param>
public sealed record CreateRecipeRequestTip(string Description);

/// <summary>
/// The DTO for a sub ingredient in a create recipe request.
/// </summary>
/// <param name="Name">The sub ingredient name.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record CreateRecipeRequestSubIngredient(string? Name, ICollection<CreateRecipeRequestIngredient> Ingredients);

/// <summary>
/// The DTO for an ingredient in a create recipe request.
/// </summary>
/// <param name="Name">The ingredient name.</param>
/// <param name="Measurement">The ingredient measurement.</param>
public sealed record CreateRecipeRequestIngredient(string Name, string Measurement);

internal sealed class CreateRecipeRequestRecipeDetailsValidator : AbstractValidator<CreateRecipeRequestDetails>
{
    public CreateRecipeRequestRecipeDetailsValidator()
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

internal sealed class CreateRecipeRequestRecipeNutritionValidator : AbstractValidator<CreateRecipeRequestNutrition>
{
    public CreateRecipeRequestRecipeNutritionValidator()
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

internal sealed class CreateRecipeRequestSubIngredientValidator : AbstractValidator<CreateRecipeRequestSubIngredient>
{
    public CreateRecipeRequestSubIngredientValidator()
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

internal sealed class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.Description)
            .MaximumLength(255)
            .When(request => request.Description != null);

        RuleFor(request => request.Details)
            .NotNull()
            .SetValidator(new CreateRecipeRequestRecipeDetailsValidator());

        RuleFor(request => request.Nutrition)
            .NotNull()
            .SetValidator(new CreateRecipeRequestRecipeNutritionValidator());

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
            .SetValidator(new CreateRecipeRequestSubIngredientValidator());
    }
}

/// <summary>
/// The handler that handles a recipe creation request.
/// </summary>
public sealed class CreateRecipeHandler
{
    private readonly MealPlannerContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IUrlGenerator _urlGenerator;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    /// <summary>
    /// Creates a <see cref="CreateRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="urlGenerator">A URL generator.</param>
    /// <param name="imageProcessingOptions">The image processing configuration of the application.</param>
    public CreateRecipeHandler(MealPlannerContext dbContext, IUserContext userContext, IUrlGenerator urlGenerator,
        IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _urlGenerator = urlGenerator;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    /// <summary>
    /// Handles the database operations necessary to create a recipe.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns an integer specifying the id of the created recipe.
    /// </returns>
    /// <exception cref="RecipeValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="UniqueConstraintViolationException">Is thrown if a unique constraint violation occurs.</exception>
    public async Task<int> HandleAsync(CreateRecipeRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var recipeCount = await _dbContext.Recipe
            .CountAsync(r => r.ApplicationUserId == _userContext.UserId, cancellationToken);

        var validationErrors = ValidateRecipe(request, recipeCount);

        if (validationErrors.Length != 0)
        {
            throw new RecipeValidationException(validationErrors);
        }

        var recipe = FromDto(request, _userContext.UserId);

        if (image != null)
        {
            var randomFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(_imageProcessingOptions.TempImageStoragePath, randomFileName);
            var filePath = Path.Combine(_imageProcessingOptions.ImageStoragePath, randomFileName);

            var recipeImage = new Image
            {
                StorageFileName = randomFileName,
                DisplayFileName = WebUtility.HtmlEncode(image.FileName),
                ImagePath = filePath,
                ImageUrl = _urlGenerator.GenerateUrl("GetImageByFileName", new { fileName = randomFileName }),
                ManageableEntityId = recipe.Id,
                ManageableEntity = recipe
            };
            recipe.Image = recipeImage;

            _dbContext.Recipe.Add(recipe);
            await _dbContext.SaveChangesSaveImageAsync(image, tempFilePath, filePath,
                _imageProcessingOptions.PermittedExtensions, _imageProcessingOptions.ImageSizeLimit, cancellationToken);

            return recipe.Id;
        }

        _dbContext.Recipe.Add(recipe);
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

        return recipe.Id;
    }

    private static Error[] ValidateRecipe(CreateRecipeRequest request, int recipeCount)
    {
        List<Error> errors = [];

        if (recipeCount > RecipeErrors.MaxRecipesCount)
        {
            errors.Add(RecipeErrors.MaxRecipes());
        }

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

    private static Recipe FromDto(CreateRecipeRequest request, int userId)
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

        var recipe = new Recipe
        {
            Title = request.Title,
            Description = request.Description,
            RecipeDetails = recipeDetails,
            RecipeNutrition = recipeNutrition,
            ApplicationUserId = userId
        };

        foreach (var directionDto in request.Directions)
        {
            var direction = new Direction() { Number = directionDto.Number, Description = directionDto.Description };

            recipe.Directions.Add(direction);
        }

        foreach (var tipDto in request.Tips)
        {
            var tip = new Tip() { Description = tipDto.Description };

            recipe.Tips.Add(tip);
        }

        foreach (var subIngredientDto in request.SubIngredients)
        {
            var subIngredient = new SubIngredient() { Name = subIngredientDto.Name };

            foreach (var ingredientDto in subIngredientDto.Ingredients)
            {
                var ingredient = new Ingredient() { Name = ingredientDto.Name, Measurement = ingredientDto.Measurement };

                subIngredient.Ingredients.Add(ingredient);
            }

            recipe.SubIngredients.Add(subIngredient);
        }

        return recipe;
    }
}