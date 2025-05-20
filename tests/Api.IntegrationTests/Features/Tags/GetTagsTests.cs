using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Tags;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Features.Tags;

public class GetTagsTests : TestBaseAuthentication
{
    private const string Uri = "api/tags";

    public GetTagsTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    [Fact]
    public async Task GetAsync_ShouldReturn200OkWithContentTypeJsonWithTags_WhenTagsExist()
    {
        var response = await HttpClient.GetAsync(Uri,
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<GetTagsDto>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Equal(7, body.Count);
    }
}