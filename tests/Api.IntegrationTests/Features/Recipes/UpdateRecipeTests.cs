using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Features.Recipes;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;

namespace Api.IntegrationTests.Features.Recipes;

public class UpdateRecipeTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/recipes";

    public UpdateRecipeTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsInvalid()
    {
        const int recipeId = 11;
        var json = new StringContent(JsonSerializer.Serialize("Invalid request"), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsNull()
    {
        const int recipeId = 11;
        var json = new StringContent(JsonSerializer.Serialize<object?>(null), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIdDoesNotMatchRouteId()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{12}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIsInvalid()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(-1, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanSixDirections()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number one."),
                new(0, 2, "Test recipe direction number two."),
                new(0, 3, "Test recipe direction number three."),
                new(0, 4, "Test recipe direction number four."),
                new(0, 5, "Test recipe direction number five."),
                new(0, 6, "Test recipe direction number six."),
                new(0, 6, "Test recipe direction number six.")
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeTips()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number one.")
            },
            [
                new(0, "Test tip one."),
                new(0, "Test tip two."),
                new(0, "Test tip three."),
                new(0, "Test tip number four."),
            ],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanFiveSubIngredients()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number one.")
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, "Sub Ingredient One", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, "Sub Ingredient Two", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, "Sub Ingredient Three", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, "Sub Ingredient Four", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, "Sub Ingredient Five", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, "Sub Ingredient Six", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanTenIngredientsInASubIngredient()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number one.")
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, "Sub Ingredient One", new List<UpdateRecipeRequestIngredient>
                {
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                    new(0, "Test Name", "Test Measurement"),
                })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestDirectionsNumberDoNotStartAtOne()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 2, "Test recipe direction number two."),
                new(0, 3, "Test recipe direction number three."),
                new(0, 4, "Test recipe direction number four."),
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestDirectionsNotSequential()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number One."),
                new(0, 3, "Test recipe direction number three."),
                new(0, 4, "Test recipe direction number four."),
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestSubIngredientCountIsOneAndNameIsNotNull()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number One.")
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, "Sub Ingredient One", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestSubIngredientCountIsGreaterThanOneAndNameIsNull()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection>
            {
                new(0, 1, "Test recipe direction number One.")
            },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, "Sub Ingredient One", new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") }),
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn404NotFoundWithContentTypeProblemJsonWithProblemDetails_WhenRecipeDoesNotExist()
    {
        const int recipeId = 1;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasImageAndRecipeHadImageAndIsSuccessful()
    {
        const int recipeId = 11;
        const string fileName = "pumpkin_muffin.jpg";
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasImageAndRecipeHadNoImageAndIsSuccessful()
    {
        const int recipeId = 16;
        const string fileName = "pumpkin_muffin.jpg";
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasNoImageAndRecipeHadImageAndIsSuccessful()
    {
        const int recipeId = 11;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        UpdateAsync_ShouldReturn200OkWithEmptyBody_WhenRequestHasNoImageAndRecipeHadNoImageAndIsSuccessful()
    {
        const int recipeId = 16;
        var recipe = new UpdateRecipeRequest(
            recipeId,
            "Test Recipe",
            null,
            new UpdateRecipeRequestDetails(null, null, null),
            new UpdateRecipeRequestNutrition(null, null, null, null),
            new List<UpdateRecipeRequestDirection> { new(0, 1, "Test recipe direction number one.") },
            [],
            new List<UpdateRecipeRequestSubIngredient>
            {
                new(0, null, new List<UpdateRecipeRequestIngredient> { new(0, "Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PutAsync($"{Uri}/{recipeId}", content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.Empty(body);
    }
}