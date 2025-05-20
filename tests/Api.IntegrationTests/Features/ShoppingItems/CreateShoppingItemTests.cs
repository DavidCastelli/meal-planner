using System.Net;
using System.Net.Mime;

using Api.Features.ShoppingItems;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.ShoppingItems;

public class CreateShoppingItemTests : TestBaseAuthentication
{
    private const string Uri = "api/shopping-items";

    public CreateShoppingItemTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIsInvalid()
    {
        var request = new CreateShoppingItemRequest("Test Name", null, null, -1);

        var response = await HttpClient.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenIsSuccessful()
    {
        var request = new CreateShoppingItemRequest("Test Name", null, null, null);

        var response = await HttpClient.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }
}