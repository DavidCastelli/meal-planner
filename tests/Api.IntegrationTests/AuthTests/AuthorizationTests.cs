using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Domain.Meals;
using Api.Features.Meals;
using Api.Features.Recipes;
using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.AuthTests;

public class AuthorizationTests : TestBaseAuthorization
{
    public AuthorizationTests(AuthorizationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        GetRecipeById_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int recipeId = 11;

        var response = await HttpClient.GetAsync($"api/manage/recipes/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateRecipe_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"api/manage/recipes/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteRecipe_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int recipeId = 11;

        var response = await HttpClient.DeleteAsync($"api/manage/recipes/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetMealById_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int mealId = 1;

        var response = await HttpClient.GetAsync($"api/manage/meals/{mealId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateMeal_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"api/manage/meals/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteMeal_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int mealId = 1;

        var response = await HttpClient.DeleteAsync($"api/manage/meals/{mealId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        ScheduleMealById_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Remove(m => m.Schedule);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response = await HttpClient.PatchAsync($"api/manage/meals/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetImageByFileName_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        var fileName = TestDataSeeder.GetTestRecipe().Image!.StorageFileName;

        var response = await HttpClient.GetAsync($"api/images/{fileName}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetShoppingItemById_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int shoppingItemId = 1;

        var response = await HttpClient.GetAsync($"api/shopping-items/{shoppingItemId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateShoppingItem_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int shoppingItemId = 1;
        var request = new UpdateShoppingItemRequest(shoppingItemId, "Test Name", null, null, null, false, false);

        var response = await HttpClient.PutAsJsonAsync($"api/shopping-items/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteShoppingItem_ShouldReturn403ForbiddenWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthorized()
    {
        const int shoppingItemId = 1;

        var response = await HttpClient.DeleteAsync($"api/shopping-items/{shoppingItemId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}