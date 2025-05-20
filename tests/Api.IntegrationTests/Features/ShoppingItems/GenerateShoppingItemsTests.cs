using System.Net;
using System.Net.Mime;

using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class GenerateShoppingItemsTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items/generate";

    public GenerateShoppingItemsTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        GenerateAsync_ShouldReturn409ConflictWithContentTypeProblemJsonWithProblemDetails_WhenGenerateAgainBeforeClearing()
    {
        await HttpClient.PostAsync(Uri, null, TestContext.Current.CancellationToken);

        var response = await HttpClient.PostAsync(Uri, null, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        GenerateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenScheduledMealsExistAndIsSuccessful()
    {
        var response = await HttpClient.PostAsync(Uri, null, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        GenerateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenScheduledMealsDoNotExistAndIsSuccessful()
    {
        await HttpClient.PatchAsync("api/manage/meals/schedule", null, TestContext.Current.CancellationToken);

        var response = await HttpClient.PostAsync(Uri, null, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }
}