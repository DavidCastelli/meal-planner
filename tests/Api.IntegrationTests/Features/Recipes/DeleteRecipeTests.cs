using System.Net;
using System.Net.Mime;

using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Recipes;

public class DeleteRecipeTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/recipes";

    public DeleteRecipeTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenRecipeDoesNotExist()
    {
        const int recipeId = 1;

        var response = await HttpClient.DeleteAsync($"{Uri}/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn409ConflictWithContentTypeProblemJsonWithProblemDetails_WhenDeletingLastRecipeBelongingToAMeal()
    {
        const int recipeId = 11;

        var response = await HttpClient.DeleteAsync($"{Uri}/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn200OkWithEmptyBody_WhenRecipeHasImageAndIsSuccessful()
    {
        const int recipeId = 15;
        const int mealId = 5;
        await HttpClient.DeleteAsync($"api/manage/meals/{mealId}", TestContext.Current.CancellationToken); // Needed to avoid 409 Conflict from deleting the last recipe of a meal

        var response = await HttpClient.DeleteAsync($"{Uri}/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        DeleteAsync_ShouldReturn200OkWithEmptyBody_WhenRecipeHasNoImageAndIsSuccessful()
    {
        const int recipeId = 16;

        var response = await HttpClient.DeleteAsync($"{Uri}/{recipeId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType);
        Assert.Empty(body);
    }
}