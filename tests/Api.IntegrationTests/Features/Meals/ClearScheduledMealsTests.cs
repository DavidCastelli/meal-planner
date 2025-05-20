using System.Net;

using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Features.Meals;

public class ClearScheduledMealsTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals/schedule";

    public ClearScheduledMealsTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task PatchAsync_ShouldReturn200OkWithEmptyBody_WhenIsSuccessful()
    {
        var response = await HttpClient.PatchAsync(Uri, null, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}