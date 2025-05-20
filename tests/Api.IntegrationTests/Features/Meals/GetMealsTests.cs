using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Features.Meals;

public class GetMealsTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/meals";

    public GetMealsTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task GetAsync_ShouldReturn200OkWithContentTypeJsonWithMeals_WhenMealsExistWithNoQueryParam()
    {
        var response = await HttpClient.GetAsync(Uri,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<GetMealsDto>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equal(10, body.Count);
    }

    [Fact]
    public async Task
        GetAsync_ShouldReturn200OkWithContentTypeJsonWithScheduledMeals_WhenMealsExistWithScheduledQueryParam()
    {
        var response = await HttpClient.GetAsync($"{Uri}?scheduled=true",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<GetMealsDto>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.InRange(body.Count, 1, 7);
    }
}