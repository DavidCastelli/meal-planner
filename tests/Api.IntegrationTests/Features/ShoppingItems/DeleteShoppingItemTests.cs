using System.Net;
using System.Net.Mime;

using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class DeleteShoppingItemTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items";

    public DeleteShoppingItemTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenShoppingItemDoesNotExist()
    {
        const int shoppingItemId = 2;

        var response = await HttpClient.DeleteAsync($"{Uri}/{shoppingItemId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn409ConflictWithContentTypeProblemJsonWithProblemDetails_WhenShoppingItemIsLocked()
    {
        var testShoppingItem = TestDataSeeder.GetTestShoppingItem();
        var request = new UpdateShoppingItemRequest(
            testShoppingItem.Id,
            testShoppingItem.Name,
            testShoppingItem.Measurement,
            testShoppingItem.Price,
            testShoppingItem.Quantity,
            testShoppingItem.IsChecked,
            true);
        var debug = await HttpClient.PutAsJsonAsync($"{Uri}/{request.Id}", request, TestContext.Current.CancellationToken);

        var response = await HttpClient.DeleteAsync($"{Uri}/{testShoppingItem.Id}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturn200OkWithEmptyBody_WhenIsSuccessful()
    {
        var testShoppingItem = TestDataSeeder.GetTestShoppingItem();
        var request = new UpdateShoppingItemRequest(
            testShoppingItem.Id,
            testShoppingItem.Name,
            testShoppingItem.Measurement,
            testShoppingItem.Price,
            testShoppingItem.Quantity,
            testShoppingItem.IsChecked,
            false);
        await HttpClient.PutAsJsonAsync($"{Uri}/{request.Id}", request, TestContext.Current.CancellationToken);

        var response = await HttpClient.DeleteAsync($"{Uri}/{testShoppingItem.Id}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}