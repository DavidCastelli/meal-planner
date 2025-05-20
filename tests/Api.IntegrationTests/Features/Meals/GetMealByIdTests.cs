using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Meals;

public class GetMealByIdTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals";

    public GetMealByIdTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task
        GetByIdAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenMealDoesNotExist()
    {
        const int mealId = 11;

        var response = await HttpClient.GetAsync($"{Uri}/{mealId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetByIdAsync_ShouldReturn200OkWithContentTypeJsonWithMeal_WhenMealExists()
    {
        var testMeal = TestDataSeeder.GetTestMeal();
        var expected = new
        {
            testMeal.Id,
            testMeal.Title,
            Image =
                new
                {
                    testMeal.Image?.Id,
                    testMeal.Image?.StorageFileName,
                    testMeal.Image?.DisplayFileName,
                    testMeal.Image?.ImageUrl
                },
            testMeal.Schedule,
            Tags = testMeal.Tags.Select(t => new { t.Id, TagType = t.Type }),
            Recipes = testMeal.Recipes.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Image?.ImageUrl,
                Details = r.RecipeDetails,
                Nutrition = r.RecipeNutrition
            }),
            testMeal.ApplicationUserId
        };

        var response = await HttpClient.GetAsync($"{Uri}/{expected.Id}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<GetMealByIdDto>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equivalent(expected, body);
    }
}