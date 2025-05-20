using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Api.Features.Recipes;
using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.IntegrationTests.Features.Recipes;

public class CreateRecipeTests : TestBaseAuthentication
{
    private const string Uri = "api/manage/recipes";

    public CreateRecipeTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsInvalid()
    {
        var json = new StringContent(JsonSerializer.Serialize("Invalid request"), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestJsonIsNull()
    {
        var json = new StringContent(JsonSerializer.Serialize<object?>(null), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestIsInvalid()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(-1, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection> { new(1, "Test recipe direction number one.") },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanSixDirections()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number one."),
                new(2, "Test recipe direction number two."),
                new(3, "Test recipe direction number three."),
                new(4, "Test recipe direction number four."),
                new(5, "Test recipe direction number five."),
                new(6, "Test recipe direction number six."),
                new(6, "Test recipe direction number six.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanThreeTips()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number one.")
            },
            [
                new("Test tip one."),
                new("Test tip two."),
                new("Test tip three."),
                new("Test tip number four.")],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanFiveSubIngredients()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number one.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new("Sub Ingredient One", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new("Sub Ingredient Two", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new("Sub Ingredient Three", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new("Sub Ingredient Four", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new("Sub Ingredient Five", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new("Sub Ingredient Six", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasMoreThanTenIngredientsInASubIngredient()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number one.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new("Sub Ingredient One", new List<CreateRecipeRequestIngredient>
                {
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement"),
                    new("Test Name", "Test Measurement")
                })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestDirectionsNumberDoNotStartAtOne()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(2, "Test recipe direction number two."),
                new(3, "Test recipe direction number three."),
                new(4, "Test recipe direction number four.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestDirectionsNotSequential()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number One."),
                new(3, "Test recipe direction number three."),
                new(4, "Test recipe direction number four.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestSubIngredientCountIsOneAndNameIsNotNull()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number One.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new("Sub Ingredient One", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn400BadRequestWithContentTypeProblemJsonWithProblemDetails_WhenRequestSubIngredientCountIsGreaterThanOneAndNameIsNull()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection>
            {
                new(1, "Test recipe direction number One.")
            },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new("Sub Ingredient One", new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") }),
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn409ConflictWithContentTypeProblemJsonWithProblemDetails_WhenRequestHasNoImageAndTitleViolatesUniqueConstraint()
    {
        var testRecipe = TestDataSeeder.GetTestRecipe();
        var recipe = new CreateRecipeRequest(
            testRecipe.Title,
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection> { new(1, "Test recipe direction number one.") },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent
        {
            { json, "data", "data.json" }
        };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenRequestHasImageAndIsSuccessful()
    {
        const string fileName = "pumpkin_muffin.jpg";
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection> { new(1, "Test recipe direction number one.") },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var testImageLocation = $"{TestDataSeeder.TestImageProcessingOptions.ImageAssetsPath}/{fileName}";
        var image = new ByteArrayContent(await File.ReadAllBytesAsync(testImageLocation, TestContext.Current.CancellationToken));
        image.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Image.Jpeg);
        var content = new MultipartFormDataContent { { json, "data", "data.json" }, { image, "image", "image.jpg" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }

    [Fact]
    public async Task
        CreateAsync_ShouldReturn201CreatedWithLocationHeaderWithEmptyBody_WhenRequestHasNoImageAndIsSuccessful()
    {
        var recipe = new CreateRecipeRequest(
            "Test Recipe",
            null,
            new CreateRecipeRequestDetails(null, null, null),
            new CreateRecipeRequestNutrition(null, null, null, null),
            new List<CreateRecipeRequestDirection> { new(1, "Test recipe direction number one.") },
            [],
            new List<CreateRecipeRequestSubIngredient>
            {
                new(null, new List<CreateRecipeRequestIngredient> { new("Test Name", "Test Measurement") })
            });
        var json = new StringContent(JsonSerializer.Serialize(recipe), Encoding.UTF8, MediaTypeNames.Application.Json);
        var content = new MultipartFormDataContent { { json, "data", "data.json" } };

        var response = await HttpClient.PostAsync(Uri, content, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(response.Headers.Location);
        Assert.Empty(body);
    }
}