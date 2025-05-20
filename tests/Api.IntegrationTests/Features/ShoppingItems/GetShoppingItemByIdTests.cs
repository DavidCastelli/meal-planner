using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class GetShoppingItemByIdTests : TestBaseAuthentication
{
    public const string Uri = "api/shopping-items";

    public GetShoppingItemByIdTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task
        GetByIdAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenShoppingItemDoesNotExist()
    {
        const int shoppingItemId = 2;

        var response = await HttpClient.GetAsync($"{Uri}/{shoppingItemId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current
            .CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturn200OkWithContentTypeJsonWithShoppingItem_WhenShoppingItemExists()
    {
        var testShoppingItem = TestDataSeeder.GetTestShoppingItem();
        var expected = new
        {
            testShoppingItem.Id,
            testShoppingItem.Name,
            testShoppingItem.Measurement,
            testShoppingItem.Price,
            testShoppingItem.Quantity,
            testShoppingItem.IsChecked,
            testShoppingItem.IsLocked,
            testShoppingItem.IsGenerated,
            testShoppingItem.ApplicationUserId
        };

        var response = await HttpClient.GetAsync($"{Uri}/{expected.Id}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<GetShoppingItemByIdDto>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equivalent(expected, body);
    }
}