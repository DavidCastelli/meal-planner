using System.Net;
using System.Net.Http.Headers;
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

public class AuthenticationTests : TestBase
{
    public AuthenticationTests(CommonTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        GetRecipes_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        var response = await HttpClient.GetAsync("api/manage/recipes",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetRecipeById_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        const int recipeId = 11;

        var response = await HttpClient.GetAsync($"api/manage/recipes/{recipeId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateRecipe_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection> { new(1, "Test recipe direction number one.") },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync("api/manage/recipes", content,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateRecipe_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int recipeId = 11;
        var recipeUpdate = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new UpdateRecipeRequestSubIngredient(0, null,
                    new List<UpdateRecipeRequestIngredient> { new(0, "Test Ingredient", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipeUpdate), Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var request = await HttpClient.PutAsync($"api/manage/recipes/{recipeId}", content,
            TestContext.Current.CancellationToken);
        var body = await request.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, request.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteRecipe_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int recipeId = 11;

        var response = await HttpClient.DeleteAsync($"api/manage/recipes/{recipeId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetMeals_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        var response = await HttpClient.GetAsync("api/manage/meals",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetMealsById_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        const int mealId = 1;

        var response = await HttpClient.GetAsync($"api/manage/meals/{mealId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateMeal_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var meal = new CreateMealRequest("Test Meal", [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync("api/manage/meals", content,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateMeal_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int mealId = 1;
        var mealUpdate = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(mealUpdate), Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"api/manage/meals/{mealId}", content,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        ScheduleMealById_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, Schedule.None);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8, MediaTypeNames.Application.JsonPatch);

        var response = await HttpClient.PatchAsync($"api/manage/meals/{mealId}/schedule", content,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        ClearScheduledMeals_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var response = await HttpClient.PatchAsync("api/manage/meals/schedule", null,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteMeal_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int mealId = 1;

        var response = await HttpClient.DeleteAsync($"api/manage/meals/{mealId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetTags_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        var response = await HttpClient.GetAsync("api/tags",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetImageByFileName_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg));
        const string fileName = "pumpkin_muffin.jpg";

        var response = await HttpClient.GetAsync($"api/images/{fileName}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetShoppingItems_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));


        var response = await HttpClient.GetAsync("api/shopping-items",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GetShoppingItemById_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        const int shoppingItemId = 1;

        var response = await HttpClient.GetAsync($"api/shopping-items/{shoppingItemId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateShoppingItem_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var shoppingItem = new CreateShoppingItemRequest("Test Shopping Item", "1 Cup", null, null);

        var response = await HttpClient.PostAsJsonAsync("api/shopping-items", shoppingItem,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GenerateShoppingItems_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var response = await HttpClient.PostAsync("api/shopping-items/generate", null,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateShoppingItem_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int shoppingItemId = 1;
        var shoppingItemUpdate = new UpdateShoppingItemRequest(
            shoppingItemId,
            "Test Shopping Item",
            "1 Cup",
            null,
            null,
            false,
            false);

        var response = await HttpClient.PutAsJsonAsync($"api/shopping-items/{shoppingItemId}", shoppingItemUpdate,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteShoppingItem_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        const int shoppingItemId = 1;

        var response = await HttpClient.DeleteAsync($"api/shopping-items/{shoppingItemId}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        ClearShoppingItems_ShouldReturn401UnauthorizedWithContentTypeProblemJsonWithProblemDetails_WhenUserIsNotAuthenticated()
    {
        var response = await HttpClient.DeleteAsync("api/shopping-items",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}