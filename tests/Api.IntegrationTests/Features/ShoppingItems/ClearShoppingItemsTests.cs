using System.Net;
using System.Net.Mime;

using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class ClearShoppingItemsTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items";

    public ClearShoppingItemsTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        DeleteAllAsync_ShouldReturn422UnprocessableEntityWithContentTypeJsonWithProblemDetails_WhenInvalidQueryParam()
    {
        var response = await HttpClient.DeleteAsync($"{Uri}?clear=invalid", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Theory]
    [InlineData("all")]
    [InlineData("checked")]
    public async Task DeleteAllAsync_ShouldReturn200OkWithEmptyBody_WhenValidQueryParamAndIsSuccessful(string query)
    {
        var response = await HttpClient.DeleteAsync($"{Uri}?clear={query}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

}