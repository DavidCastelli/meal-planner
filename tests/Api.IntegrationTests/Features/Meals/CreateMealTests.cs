using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Meals;

public class CreateMealTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals";

    public CreateMealTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsInvalid()
    {
        var json = new StringContent(JsonSerializer.Serialize("Invalid request"), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsNull()
    {
        var json = new StringContent(JsonSerializer.Serialize<object?>(null), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIsInvalid()
    {
        var meal = new CreateMealRequest("Test Meal", [], []);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeTags()
    {
        var meal = new CreateMealRequest("Test Meal", [1, 2, 3, 4], []);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeRecipes()
    {
        var meal = new CreateMealRequest("Test Meal", [], [11, 12, 13, 14]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasDuplicateRecipeId()
    {
        var meal = new CreateMealRequest("Test Meal", [], [11, 11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasDuplicateTagId()
    {
        var meal = new CreateMealRequest("Test Meal", [1, 1], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasNonExistingRecipeId()
    {
        var meal = new CreateMealRequest("Test Meal", [], [1]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasNonExistingTagId()
    {
        var meal = new CreateMealRequest("Test Meal", [17], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn409ConflictWithContentTypeProblemDetailsWithProblemDetails_WhenRequestHasNoImageAndTitleViolatesUniqueConstraint()
    {
        var testMeal = TestDataSeeder.GetTestMeal();
        var meal = new CreateMealRequest(testMeal.Title, [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenRequestHasImageAndIsSuccessful()
    {
        const string fileName = "pumpkin_muffin.jpg";
        var meal = new CreateMealRequest("Test Meal", [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenRequestHasNoImageAndIsSuccessful()
    {
        var meal = new CreateMealRequest("Test Meal", [], [11]);
        var json = new StringContent(JsonSerializer.Serialize(meal), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }
}