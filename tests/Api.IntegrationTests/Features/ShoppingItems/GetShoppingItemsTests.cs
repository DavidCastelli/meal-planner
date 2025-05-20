using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class GetShoppingItemsTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items";

    public GetShoppingItemsTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task GetAsync_ShouldReturn200OkWithContentTypeJsonWithShoppingItems_WhenShoppingItemsExist()
    {
        var response = await HttpClient.GetAsync(Uri, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<GetShoppingItemsDto>>(TestContext.Current
            .CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}