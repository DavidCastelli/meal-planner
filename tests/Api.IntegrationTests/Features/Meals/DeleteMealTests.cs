using System.Net;
using System.Net.Mime;

using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Meals;

public class DeleteMealTests : TestBaseAuthentication
{
    private const string Uri = "/api/manage/meals";

    public DeleteMealTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenMealDoesNotExist()
    {
        const int mealId = 11;

        var response = await HttpClient.DeleteAsync($"{Uri}/{mealId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturn200OkWithEmptyBody_WhenMealHasImageAndIsSuccessful()
    {
        const int mealId = 1;

        var response = await HttpClient.DeleteAsync($"{Uri}/{mealId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturn200OkWithEmptyBody_WhenMealHasNoImageAndIsSuccessful()
    {
        const int mealId = 6;

        var response = await HttpClient.DeleteAsync($"{Uri}/{mealId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}