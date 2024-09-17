using System.Net.Mime;
using System.Text.Json;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Common.Utilities;
using Api.Domain;
using Api.Domain.Ingredients;
using Api.Domain.Recipes;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Features.RecipeManagement;

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
    [ProducesResponseType(typeof(UpdateRecipeDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, BadRequest<ValidationProblemDetails>, ValidationProblem, NotFound, Ok<UpdateRecipeDto>>> UpdateAsync(int id, IValidator<UpdateRecipeRequest> validator, UpdateRecipeHandler handler, IFormFile data, IFormFile? image, CancellationToken cancellationToken)
    {
        var request = await JsonSerializer.DeserializeAsync<UpdateRecipeRequest>(
            data.OpenReadStream(), cancellationToken: cancellationToken);

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

        var updateRecipeDto = await handler.HandleAsync(request, image, cancellationToken);

        return TypedResults.Ok(updateRecipeDto);
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
public sealed record UpdateRecipeRequest(int Id, string Title, string? Description, UpdateRecipeRequestRecipeDetails Details, UpdateRecipeRequestRecipeNutrition Nutrition, IList<UpdateRecipeRequestDirection> Directions, ICollection<UpdateRecipeRequestTip> Tips, ICollection<UpdateRecipeRequestSubIngredient> SubIngredients);

/// <summary>
/// The DTO for the recipe details in an update recipe request.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record UpdateRecipeRequestRecipeDetails(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition in an update recipe request.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record UpdateRecipeRequestRecipeNutrition(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction in an update recipe request.
/// </summary>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record UpdateRecipeRequestDirection(int Number, string Description);

/// <summary>
/// The DTO for a tip in an update recipe request.
/// </summary>
/// <param name="Description">The tip description.</param>
public sealed record UpdateRecipeRequestTip(string Description);

/// <summary>
/// The DTO for a sub ingredient in an update recipe request.
/// </summary>
/// <param name="Name">The sub ingredient name.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record UpdateRecipeRequestSubIngredient(string? Name, ICollection<UpdateRecipeRequestIngredient> Ingredients);

/// <summary>
/// The DTO for an ingredient in an update recipe request.
/// </summary>
/// <param name="Name">The ingredient name.</param>
/// <param name="Measurement">The ingredient measurement.</param>
public sealed record UpdateRecipeRequestIngredient(string Name, string Measurement);

internal sealed class UpdateRecipeRequestRecipeDetailsValidator : AbstractValidator<UpdateRecipeRequestRecipeDetails>
{
    public UpdateRecipeRequestRecipeDetailsValidator()
    {
        RuleFor(rd => rd.PrepTime)
            .GreaterThanOrEqualTo(1)
            .When(rd => rd.PrepTime != null);

        RuleFor(rd => rd.CookTime)
            .GreaterThanOrEqualTo(1)
            .When(rd => rd.CookTime != null);

        RuleFor(rd => rd.Servings)
            .GreaterThanOrEqualTo(1)
            .When(rd => rd.Servings != null);
    }
}

internal sealed class UpdateRecipeRequestRecipeNutritionValidator : AbstractValidator<UpdateRecipeRequestRecipeNutrition>
{
    public UpdateRecipeRequestRecipeNutritionValidator()
    {
        RuleFor(rn => rn.Calories)
            .GreaterThanOrEqualTo(1)
            .When(rn => rn.Calories != null);

        RuleFor(rn => rn.Fat)
            .GreaterThanOrEqualTo(1)
            .When(rn => rn.Fat != null);

        RuleFor(rn => rn.Carbs)
            .GreaterThanOrEqualTo(1)
            .When(rn => rn.Carbs != null);

        RuleFor(rn => rn.Protein)
            .GreaterThanOrEqualTo(1)
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
                .GreaterThanOrEqualTo(1);

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
    private readonly IAuthorizationService _authorizationService;
    private readonly IImageProcessingInfo _imageProcessingInfo;

    /// <summary>
    /// Creates a <see cref="UpdateRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    /// <param name="authorizationService">The authorization service.</param>
    /// <param name="imageProcessingInfo">The image processing configuration of the application.</param>
    public UpdateRecipeHandler(MealPlannerContext dbContext, IUserContext userContext, IAuthorizationService authorizationService, IImageProcessingInfo imageProcessingInfo)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _authorizationService = authorizationService;
        _imageProcessingInfo = imageProcessingInfo;
    }

    /// <summary>
    /// Handles the database operations necessary to update a recipe.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="image">The image for the request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="UpdateRecipeDto"/>.
    /// </returns>
    /// <exception cref="RecipeNotFoundException">Is thrown if the recipe could not be found in the data store.</exception>
    /// <exception cref="RecipeValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    /// <exception cref="ForbiddenException">Is thrown if the user is authenticated but lacks permission to access the resource.</exception>
    /// <exception cref="UnauthorizedException">Is thrown if the user lacks the necessary authentication credentials.</exception>
    public async Task<UpdateRecipeDto> HandleAsync(UpdateRecipeRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        var validationErrors = ValidateRecipe(request);

        if (validationErrors.Length != 0)
        {
            throw new RecipeValidationException(validationErrors);
        }

        var recipeUpdate = FromDto(request, _userContext.UserId);

        var recipe = await _dbContext.Recipes.FindAsync([recipeUpdate.Id], cancellationToken);

        if (recipe == null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(_userContext.User, recipe, "SameUserPolicy");

        if (authorizationResult.Succeeded)
        {
            recipe.Title = request.Title;
            recipe.Description = request.Description;
            recipe.RecipeDetails = recipeUpdate.RecipeDetails;
            recipe.RecipeNutrition = recipeUpdate.RecipeNutrition;

            recipe.Directions.Clear();
            recipe.Tips.Clear();
            recipe.SubIngredients.Clear();

            foreach (var directionUpdate in recipeUpdate.Directions)
            {
                recipe.Directions.Add(directionUpdate);
            }

            foreach (var tipUpdate in recipeUpdate.Tips)
            {
                recipe.Tips.Add(tipUpdate);
            }

            foreach (var subIngredientUpdate in recipeUpdate.SubIngredients)
            {
                recipe.SubIngredients.Add(subIngredientUpdate);
            }

            bool isCancellable = true;
            if (image != null)
            {
                if (recipe.ImagePath != null)
                {
                    var tempFilePath = Path.Combine(_imageProcessingInfo.TempImageStoragePath,
                        Path.GetFileName(recipe.ImagePath));
                    
                    var imageProcessingErrors = await FileHelpers.ProcessFormFileAsync(image, tempFilePath, recipe.ImagePath,
                        _imageProcessingInfo.PermittedExtensions, _imageProcessingInfo.ImageSizeLimit, true);

                    if (imageProcessingErrors.Length != 0)
                    {
                        throw new ImageProcessingException(imageProcessingErrors);
                    }
                    
                    isCancellable = false;
                }
                else
                {
                    var randomFileName = Path.GetRandomFileName();
                    var tempFilePath = Path.Combine(_imageProcessingInfo.TempImageStoragePath, randomFileName);
                    var filePath = Path.Combine(_imageProcessingInfo.ImageStoragePath, randomFileName);
                    recipe.ImagePath = filePath;

                    var imageProcessingErrors = await FileHelpers.ProcessFormFileAsync(image, randomFileName, filePath,
                        _imageProcessingInfo.PermittedExtensions, _imageProcessingInfo.ImageSizeLimit);

                    if (imageProcessingErrors.Length != 0)
                    {
                        throw new ImageProcessingException(imageProcessingErrors);
                    }
                    
                    isCancellable = false;
                }
            }
            else
            {
                if (recipe.ImagePath != null)
                {
                    File.Delete(recipe.ImagePath);
                    recipe.ImagePath = null;
                    isCancellable = false;
                }
            }

            await _dbContext.SaveChangesAsync(isCancellable ? cancellationToken : CancellationToken.None);
            return ToDto(recipe);
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

        var directions = request.Directions;

        bool isUnique = directions.Select(d => d.Number).Distinct().Count() == directions.Count;

        if (!isUnique)
        {
            errors.Add(RecipeErrors.DirectionNumberNotUnique());
        }

        var subIngredients = request.SubIngredients;

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

    private static UpdateRecipeDto ToDto(Recipe recipe)
    {
        var recipeDetailsDto = new UpdateRecipeDetailsDto(recipe.RecipeDetails.PrepTime, recipe.RecipeDetails.CookTime, recipe.RecipeDetails.Servings);
        var recipeNutritionDto = new UpdateRecipeNutritionDto(recipe.RecipeNutrition.Calories, recipe.RecipeNutrition.Fat, recipe.RecipeNutrition.Carbs, recipe.RecipeNutrition.Protein);

        var directionDtos = new List<UpdateDirectionDto>();

        foreach (var direction in recipe.Directions)
        {
            var directionDto = new UpdateDirectionDto(direction.Number, direction.Description);
            directionDtos.Add(directionDto);
        }

        var tipDtos = new List<UpdateTipDto>();

        foreach (var tip in recipe.Tips)
        {
            var tipDto = new UpdateTipDto(tip.Description);
            tipDtos.Add(tipDto);
        }

        var subIngredientDtos = new List<UpdateSubIngredientDto>();

        foreach (var subIngredient in recipe.SubIngredients)
        {
            var ingredientDtos = new List<UpdateIngredientDto>();
            foreach (var ingredient in subIngredient.Ingredients)
            {
                var ingredientDto = new UpdateIngredientDto(ingredient.Name, ingredient.Measurement);
                ingredientDtos.Add(ingredientDto);
            }

            var subIngredientDto = new UpdateSubIngredientDto(subIngredient.Name, ingredientDtos);

            subIngredientDtos.Add(subIngredientDto);
        }

        return new UpdateRecipeDto(recipe.Id, recipe.Title, recipe.Description, recipeDetailsDto, recipeNutritionDto, directionDtos, tipDtos, subIngredientDtos);
    }
}

/// <summary>
/// The DTO to return to the client after updating a recipe.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Details">The recipe details.</param>
/// <param name="Nutrition">The recipe nutrition.</param>
/// <param name="Directions">A list of directions belonging to the recipe</param>
/// <param name="Tips">A collection of tips belonging to the recipe.</param>
/// <param name="SubIngredients">A collection of sub ingredients belonging to the recipe.</param>
public sealed record UpdateRecipeDto(int Id, string Title, string? Description, UpdateRecipeDetailsDto Details, UpdateRecipeNutritionDto Nutrition, IList<UpdateDirectionDto> Directions, ICollection<UpdateTipDto> Tips, ICollection<UpdateSubIngredientDto> SubIngredients);

/// <summary>
/// The DTO for recipe details to return to the client.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record UpdateRecipeDetailsDto(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition to return to the client.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record UpdateRecipeNutritionDto(int? Calories, int? Fat, int? Carbs, int? Protein);

/// <summary>
/// The DTO for a direction to return to the client.
/// </summary>
/// <param name="Number">The direction number.</param>
/// <param name="Description">The direction description.</param>
public sealed record UpdateDirectionDto(int Number, string Description);

/// <summary>
/// The DTO for a tip to return to the client.
/// </summary>
/// <param name="Description">The tip description.</param>
public sealed record UpdateTipDto(string Description);

/// <summary>
/// The DTO for a sub ingredient to return to the client.
/// </summary>
/// <param name="Name">The name of the sub ingredient.</param>
/// <param name="Ingredients">A collection of ingredients belonging to the sub ingredient.</param>
public sealed record UpdateSubIngredientDto(string? Name, ICollection<UpdateIngredientDto> Ingredients);

/// <summary>
/// The DTO for an ingredient to return to the client.
/// </summary>
/// <param name="Name">The name of the ingredient.</param>
/// <param name="Measurement">The measurement of the ingredient.</param>
public sealed record UpdateIngredientDto(string Name, string Measurement);