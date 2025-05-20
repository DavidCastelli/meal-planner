using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Domain.Meals;
using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Meals;

public class ScheduleMealByIdTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals";

    public ScheduleMealByIdTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        PatchAsync_ShouldReturn422UnprocessableEntityWithContentTypeJsonWithProblemDetails_WhenJsonPatchDocumentDoesNotHaveExactlyOneOperation()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Remove(m => m.Schedule);
        document.Add(m => m.Schedule, Schedule.Monday);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        PatchAsync_ShouldReturn422UnprocessableEntityWithContentTypeJsonWithProblemDetails_WhenJsonPatchDocumentOperationIsNotAddOrRemove()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Replace(m => m.Schedule, Schedule.Monday);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        PatchAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenJsonPatchDocumentIsInvalid()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument();
        document.Add("/schedule", "Invalid Value");
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        PatchAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenMealDoesNotExist()
    {
        const int mealId = 11;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, Schedule.Monday);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task PatchAsync_ShouldReturn200OkWithEmptyBody_WhenAddOperationWithValueSameAsCurrentIsSuccessful()
    {
        const int mealId = 1;
        var value = TestDataSeeder.GetTestMeal().Schedule;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, value);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task PatchAsync_ShouldReturn200OkWithEmptyBody_WhenAddOperationWithValueNoneIsSuccessful()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, Schedule.None);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task PatchAsync_ShouldReturn200OkWithEmptyBody_WhenRemoveOperationIsSuccessful()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Remove(m => m.Schedule);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        PatchAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenMoreThanOneMealScheduledOnSameDay()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, Schedule.Monday);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);

        HttpResponseMessage? response = null;
        ProblemDetails? body = null;
        for (int i = 0; i < 2; i++)
        {
            response = await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
            body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest) break;
        }

        Assert.Equal(HttpStatusCode.BadRequest, response?.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response?.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task PatchAsync_ShouldReturn200OkWithEmptyBody_WhenAddOperationIsSuccessful()
    {
        const int mealId = 1;
        var document = new JsonPatchDocument<ScheduleMealByIdRequest>();
        document.Add(m => m.Schedule, Schedule.Monday);
        var content = new StringContent(JsonSerializer.Serialize(document.Operations), Encoding.UTF8,
            MediaTypeNames.Application.JsonPatch);
        await HttpClient.PatchAsync($"{Uri}/schedule", null, TestContext.Current.CancellationToken);

        var response =
            await HttpClient.PatchAsync($"{Uri}/{mealId}/schedule", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}