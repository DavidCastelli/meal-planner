using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Recipes;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Features.Recipes;

public class GetRecipesTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/recipes";

    public GetRecipesTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task GetAsync_ShouldReturn200OkWithContentTypeJsonWithRecipes_WhenRecipesExist()
    {
        var response = await HttpClient.GetAsync(Uri,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<GetRecipesDto>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equal(10, body.Count);
    }
}