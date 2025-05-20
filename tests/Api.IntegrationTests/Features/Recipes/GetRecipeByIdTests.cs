using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Recipes;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Recipes;

public class GetRecipeByIdTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/recipes";

    public GetRecipeByIdTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task
        GetByIdAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenRecipeDoesNotExist()
    {
        const int recipeId = 1;

        var response = await HttpClient.GetAsync($"{Uri}/{recipeId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturn200OkWithContentTypeJsonWithRecipe_WhenRecipeExists()
    {
        var testRecipe = TestDataSeeder.GetTestRecipe();
        var expected = new
        {
            testRecipe.Id,
            testRecipe.Title,
            Image =
                new
                {
                    testRecipe.Image?.Id,
                    testRecipe.Image?.StorageFileName,
                    testRecipe.Image?.DisplayFileName,
                    testRecipe.Image?.ImageUrl
                },
            testRecipe.Description,
            Details = testRecipe.RecipeDetails,
            Nutrition = testRecipe.RecipeNutrition,
            testRecipe.Directions,
            testRecipe.Tips,
            Meals = testRecipe.Meals.Select(m => new { m.Id, m.Title }),
            testRecipe.SubIngredients,
            testRecipe.ApplicationUserId
        };

        var response = await HttpClient.GetAsync($"{Uri}/{expected.Id}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<GetRecipeByIdDto>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equivalent(expected, body);
    }
}