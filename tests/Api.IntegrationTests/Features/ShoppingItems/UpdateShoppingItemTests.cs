using System.Net;
using System.Net.Mime;

using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class UpdateShoppingItemTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items";

    public UpdateShoppingItemTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIdDoesNotMatchRouteId()
    {
        const int shoppingItemId = 1;
        var request = new UpdateShoppingItemRequest(2, "Test Name", null, null, null, false, false);

        var response = await HttpClient.PutAsJsonAsync($"{Uri}/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJson_WhenRequestIsInvalid()
    {
        const int shoppingItemId = 1;
        var request = new UpdateShoppingItemRequest(shoppingItemId, "Test Name", null, null, -1, false, false);

        var response = await HttpClient.PutAsJsonAsync($"{Uri}/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenShoppingItemDoesNotExist()
    {
        const int shoppingItemId = 2;
        var request = new UpdateShoppingItemRequest(shoppingItemId, "Test Name", null, null, null, false, false);

        var response = await HttpClient.PutAsJsonAsync($"{Uri}/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn409ConflictWithContentTypeProblemJsonWithProblemDetails_WhenLockingGeneratedShoppingItem()
    {
        const int shoppingItemId = 2;
        var request = new UpdateShoppingItemRequest(shoppingItemId, "Test Name", null, null, null, false, true);
        await HttpClient.PostAsync($"{Uri}/generate", null, TestContext.Current.CancellationToken);

        var response = await HttpClient.PutAsJsonAsync($"{Uri}/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenIsSuccessful()
    {
        const int shoppingItemId = 1;
        var request = new UpdateShoppingItemRequest(shoppingItemId, "Test Name", null, null, null, false, false);

        var response = await HttpClient.PutAsJsonAsync($"{Uri}/{shoppingItemId}", request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}