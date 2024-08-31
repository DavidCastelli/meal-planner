using System.Net.Mime;

using Api.Common;
using Api.Common.Exceptions;
using Api.Common.Interfaces;
using Api.Domain;
using Api.Domain.Ingredients;
using Api.Domain.Recipes;
using Api.Infrastructure;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.RecipeManagement;

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
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="Results{TResult1, TResult2, TResult3, TResult4}"/> object.
    /// </returns>
    [HttpPost("/api/manage/recipes")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(CreateRecipeDto), StatusCodes.Status201Created, "application/json")]
    [Tags("Manage Recipes")]
    public async Task<Results<UnauthorizedHttpResult, ValidationProblem, BadRequest, Created<CreateRecipeDto>>> CreateAsync(
        IValidator<CreateRecipeRequest> validator, CreateRecipeHandler handler, CreateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var createRecipeDto = await handler.HandleAsync(request, cancellationToken);

        var location = Url.Action(nameof(CreateAsync), new { id = createRecipeDto.Id }) ?? $"/{createRecipeDto.Id}";
        return TypedResults.Created(location, createRecipeDto);
    }
}

/// <summary>
/// The request for recipe creation.
/// </summary>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Description">The description of the recipe.</param>
/// <param name="Image">An url to the image of the recipe.</param>
/// <param name="Details">The details of the recipe.</param>
/// <param name="Nutrition">The nutrition of the recipe.</param>
/// <param name="Directions">A list of directions belonging to the recipe.</param>
/// <param name="Tips">A list of tips belonging to the recipe.</param>
/// <param name="SubIngredients">A list of sub ingredients belonging to the recipe.</param>
public sealed record CreateRecipeRequest(string Title, string? Description, string? Image, CreateRecipeRequestRecipeDetails Details, CreateRecipeRequestRecipeNutrition Nutrition, IList<CreateRecipeRequestDirection> Directions, ICollection<CreateRecipeRequestTip> Tips, ICollection<CreateRecipeRequestSubIngredient> SubIngredients);

/// <summary>
/// The DTO for the recipe details in a create recipe request.
/// </summary>
/// <param name="PrepTime">The prep time.</param>
/// <param name="CookTime">The cook time.</param>
/// <param name="Servings">The number of servings.</param>
public sealed record CreateRecipeRequestRecipeDetails(int? PrepTime, int? CookTime, int? Servings);

/// <summary>
/// The DTO for the recipe nutrition in a create recipe request.
/// </summary>
/// <param name="Calories">The number of calories.</param>
/// <param name="Fat">The amount of fat.</param>
/// <param name="Carbs">The amount of carbs.</param>
/// <param name="Protein">The amount of protein.</param>
public sealed record CreateRecipeRequestRecipeNutrition(int? Calories, int? Fat, int? Carbs, int? Protein);

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

internal sealed class CreateRecipeRequestRecipeDetailsValidator : AbstractValidator<CreateRecipeRequestRecipeDetails>
{
    public CreateRecipeRequestRecipeDetailsValidator()
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

internal sealed class CreateRecipeRequestRecipeNutritionValidator : AbstractValidator<CreateRecipeRequestRecipeNutrition>
{
    public CreateRecipeRequestRecipeNutritionValidator()
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

        RuleFor(request => request.Image)
            .MaximumLength(255)
            .When(request => request.Image != null);

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

    /// <summary>
    /// Creates a <see cref="CreateRecipeHandler"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userContext">The user context.</param>
    public CreateRecipeHandler(MealPlannerContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Handles the database operations necessary to create a recipe.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>
    /// A task which represents the asynchronous write operation.
    /// The result of the task upon completion returns a <see cref="CreateRecipeDto"/>.
    /// </returns>
    /// <exception cref="RecipeValidationException">Is thrown if validation fails on the <paramref name="request"/>.</exception>
    public async Task<CreateRecipeDto> HandleAsync(CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var errors = ValidateRecipe(request);

        if (errors.Length != 0)
        {
            throw new RecipeValidationException(errors);
        }

        var recipe = FromDto(request, _userContext.UserId);

        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(recipe);
    }

    private static Error[] ValidateRecipe(CreateRecipeRequest request)
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

        var recipe = new Recipe()
        {
            Title = request.Title,
            Description = request.Description,
            Image = request.Image,
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

    private static CreateRecipeDto ToDto(Recipe recipe) =>
        new(recipe.Id, recipe.Title, recipe.Image);
}

/// <summary>
/// The DTO to return to the client after recipe creation.
/// </summary>
/// <param name="Id">The id of the recipe.</param>
/// <param name="Title">The title of the recipe.</param>
/// <param name="Image">An url to the image of the recipe.</param>
public sealed record CreateRecipeDto(int Id, string Title, string? Image);