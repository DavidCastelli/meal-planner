using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Domain.Meals;
using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Meals;

public class UpdateMealTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals";

    public UpdateMealTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsInvalid()
    {
        const int mealId = 1;
        var json = new StringContent(JsonSerializer.Serialize("Invalid request"), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsNull()
    {
        const int mealId = 1;
        var json = new StringContent(JsonSerializer.Serialize<object?>(null), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIdDoesNotMatchRouteId()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/2", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIsInvalid()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], []);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenMealDoesNotExist()
    {
        const int mealId = 11;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeTags()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [1, 2, 3, 4], []);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeRecipes()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11, 12, 13, 14]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasDuplicateRecipeId()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11, 11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasDuplicateTagId()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [1, 1], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasNonExistingRecipeId()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [1]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasNonExistingTagId()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [17], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasImageAndMealHasImageAndIsSuccessful()
    {
        const int mealId = 1;
        const string fileName = "pumpkin_muffin.jpg";
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasImageAndMealHasNoImageAndIsSuccessful()
    {
        const int mealId = 6;
        const string fileName = "pumpkin_muffin.jpg";
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasNoImageAndMealHasImageAndIsSuccessful()
    {
        const int mealId = 1;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasNoImageAndMealHasNoImageAndIsSuccessful()
    {
        const int mealId = 6;
        var meal = new UpdateMealRequest(mealId, "Test Meal", Schedule.None, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{mealId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}