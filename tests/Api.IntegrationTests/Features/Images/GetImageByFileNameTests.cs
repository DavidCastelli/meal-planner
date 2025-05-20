using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

using Api.Features.Meals;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Images;

public class GetImageByFileNameTests : TestBaseAuthentication
{
    private const string Uri = "api/images";

    public GetImageByFileNameTests(AuthenticationTestFixture fixture) : base(fixture)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg));
    }

    [Fact]
    public async Task
        GetByFileNameAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenImageDoesNotExist()
    {
        const string fileName = "does-not-exist.jpg";

        var response = await HttpClient.GetAsync($"{Uri}/{fileName}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetByFileNameAsync_ShouldReturn200OkWithContentTypeImageJpegWithImage_WhenImageExists()
    {
        var fileName = TestDataSeeder.GetTestRecipe().Image!.StorageFileName;

        var response = await HttpClient.GetAsync($"{Uri}/{fileName}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Image.Jpeg, response.Content.Headers.ContentType?.MediaType);
        Assert.NotEmpty(body);
    }
}