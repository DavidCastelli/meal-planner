using Api.Domain.Images;
using Api.Domain.ManageableEntities;
using Api.Domain.Meals;
using Api.Domain.Recipes;
using Api.Domain.ShoppingItems;
using Api.Domain.Tags;
using Api.Infrastructure;
using Api.Infrastructure.Identity;
using Api.IntegrationTests.Infrastructure.Extensions;

using Bogus;
using Bogus.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Api.IntegrationTests.Infrastructure;

public sealed class TestDataSeeder : IDisposable
{
    private readonly Faker _faker;
    private readonly TestWebAppFactory _webAppFactory;
    private readonly IServiceScope _scope;

    private readonly List<ApplicationUser> _users = [];
    private readonly List<string> _userPasswords = [];
    private readonly List<Recipe> _recipes = [];
    private readonly List<Meal> _meals = [];
    private readonly List<ShoppingItem> _shoppingItems = [];

    public TestImageProcessingOptions TestImageProcessingOptions { get; }

    private static readonly string[] TestImageNames =
    [
        "egg_and_cheese_frittata_one.jpg",
        "eggs_in_purgatory_cooking_one.jpg",
        "pasta_with_rapini.jpg",
        "pumpkin_muffin.jpg",
        "veggie_burger.jpg",
        "tomato_pepper_soup.jpg",
        "roasted_salmon_with_rice.jpg",
        "seasoned_veggies.jpg",
        "potato_soup.jpg",
        "poke_bowl_with_tuna.jpg"
    ];

    public TestDataSeeder(TestWebAppFactory webAppFactory)
    {
        _webAppFactory = webAppFactory;
        _scope = _webAppFactory.Services.CreateScope();
        TestImageProcessingOptions = _scope.ServiceProvider.GetRequiredService<IOptions<TestImageProcessingOptions>>().Value;
        _faker = new Faker();
    }

    public async Task SeedUsersAsync()
    {
        var scope = _webAppFactory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedUsersAsync(userManager);
        scope.Dispose();
    }

    public async Task SeedAsync()
    {
        var scope = _webAppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MealPlannerContext>();

        await SeedRecipesAsync(dbContext);
        await SeedMealsAsync(dbContext);
        await SeedImagesAsync(dbContext);
        await SeedShoppingItemsAsync(dbContext);

        await dbContext.SaveChangesAsync();

        scope.Dispose();
    }

    private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var usersFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.UserName, (_, usr) => usr.Email);

        _users.AddRange(usersFaker.Generate(2));

        foreach (var user in _users)
        {
            var password = _faker.Internet.PasswordCustom(6, 10);
            _userPasswords.Add(password);
            await userManager.CreateAsync(user, password);
        }
    }

    private async Task SeedImagesAsync(MealPlannerContext dbContext)
    {
        List<Image> testImages = [];
        for (int i = 0; i < 10; i++)
        {
            var testImageName = TestImageNames[i];
            var testImage = i < 5 ? CreateTestImage(testImageName, GetTestMeal(i), i + 1)
                : CreateTestImage(testImageName, GetTestRecipe(i - 5), i + 6);

            testImages.Add(testImage);
        }

        await dbContext.Image.AddRangeAsync(testImages);
    }

    private async Task SeedRecipesAsync(MealPlannerContext dbContext)
    {
        var recipeDetailsFaker = new Faker<RecipeDetails>()
            .RuleFor(rd => rd.PrepTime, f => f.Random.Int(0, 300).OrNull(f))
            .RuleFor(rd => rd.CookTime, f => f.Random.Int(0, 300).OrNull(f))
            .RuleFor(rd => rd.Servings, f => f.Random.Int(0, 12).OrNull(f));

        var recipeNutritionFaker = new Faker<RecipeNutrition>()
            .RuleFor(rn => rn.Calories, f => f.Random.Int(0, 1000).OrNull(f))
            .RuleFor(rn => rn.Fat, f => f.Random.Int(0, 50).OrNull(f))
            .RuleFor(rn => rn.Protein, f => f.Random.Int(0, 50).OrNull(f))
            .RuleFor(rn => rn.Carbs, f => f.Random.Int(0, 100).OrNull(f));

        var directionNumber = 0;
        var directionsFaker = new Faker<Direction>()
            .RuleFor(d => d.Number, _ => ++directionNumber)
            .RuleFor(d => d.Description, f => f.Lorem.Paragraph().ClampLength(max: 255));

        var tipsFaker = new Faker<Tip>()
            .RuleFor(t => t.Description, f => f.Lorem.Paragraph().ClampLength(max: 255));

        var ingredientsFaker = new Faker<Ingredient>()
            .RuleFor(i => i.Name, f => f.Lorem.Lines(1).ClampLength(max: 20))
            .RuleFor(i => i.Measurement, f => f.Lorem.Lines(1).ClampLength(max: 20));

        var subIngredientsFaker = new Faker<SubIngredient>()
            .RuleFor(si => si.Name, f => f.Lorem.Lines(1).ClampLength(max: 20))
            .RuleForList(si => si.Ingredients, _ => ingredientsFaker.GenerateBetween(1, 10));

        var recipesFaker = new Faker<Recipe>()
            .RuleFor(r => r.Title, f => f.Lorem.Sentence().ClampLength(max: 20))
            .RuleFor(r => r.Description, f => f.Lorem.Paragraph().ClampLength(max: 255).OrNull(f))
            .RuleFor(r => r.RecipeDetails, _ => recipeDetailsFaker.Generate())
            .RuleFor(r => r.RecipeNutrition, _ => recipeNutritionFaker.Generate())
            .RuleForList(r => r.Directions, _ => directionsFaker.GenerateBetween(1, 6))
            .RuleForList(r => r.Tips, _ => tipsFaker.GenerateBetween(0, 3))
            .RuleForList(r => r.SubIngredients, _ => subIngredientsFaker.GenerateBetween(1, 5))
            .RuleFor(r => r.ApplicationUserId, 1)
            .FinishWith((_, r) =>
            {
                if (r.SubIngredients.Count == 1)
                {
                    r.SubIngredients.First().Name = null;
                }
            });

        _recipes.AddRange(recipesFaker.Generate(10));
        await dbContext.Recipe.AddRangeAsync(_recipes);
    }

    private async Task SeedMealsAsync(MealPlannerContext dbContext)
    {
        List<Schedule> scheduled = [];
        List<TagType> tagged = [];

        List<Tag> tags =
        [
            new() { Id = 1, Type = TagType.Carnivore },
            new() { Id = 2, Type = TagType.Vegetarian },
            new() { Id = 3, Type = TagType.Vegan },
            new() { Id = 4, Type = TagType.Breakfast },
            new() { Id = 5, Type = TagType.Lunch },
            new() { Id = 6, Type = TagType.Dinner },
            new() { Id = 7, Type = TagType.Supper }
        ];

        dbContext.AttachRange(tags);

        var mealsFaker = new Faker<Meal>()
            .RuleFor(m => m.Title, f => f.Lorem.Sentence().ClampLength(max: 20))
            .RuleFor(m => m.Schedule, f =>
            {
                var schedule = f.PickRandomWithout(scheduled.ToArray());
                if (schedule != Schedule.None)
                {
                    scheduled.Add(schedule);
                }
                return schedule;
            })
            .RuleForList(m => m.Tags, f =>
            {
                var tagCount = f.Random.Int(0, 3);
                for (int i = 0; i < tagCount; i++)
                {
                    var tagType = f.PickRandomWithout(tagged.ToArray());
                    tagged.Add(tagType);
                }

                var testTags = tags.Where(t => tagged.Contains(t.Type)).ToList();
                tagged.Clear();
                return testTags;
            })
            .RuleFor(m => m.ApplicationUserId, 1);

        _meals.AddRange(mealsFaker.Generate(10));

        if (!_meals.Exists(m => m.Schedule != Schedule.None))
        {
            _meals[0].Schedule = Schedule.Monday;
        }

        var index1 = 4;
        var index2 = 4;
        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                var recipe = GetTestRecipe(i);

                var meal = GetTestMeal(i);
                meal.Recipes.Add(recipe);
            }
            else if (i < 8)
            {
                var recipe1 = GetTestRecipe(index1);
                var recipe2 = GetTestRecipe(index1 + 1);

                var meal = GetTestMeal(i);
                meal.Recipes.Add(recipe1);
                meal.Recipes.Add(recipe2);

                index1 += 2;
            }
            else
            {
                var recipe1 = GetTestRecipe(index2);
                var recipe2 = GetTestRecipe(index2 + 1);
                var recipe3 = GetTestRecipe(index2 + 2);

                var meal = GetTestMeal(i);
                meal.Recipes.Add(recipe1);
                meal.Recipes.Add(recipe2);
                meal.Recipes.Add(recipe3);

                index2 += 3;
            }
        }

        await dbContext.Meal.AddRangeAsync(_meals);
    }

    private async Task SeedShoppingItemsAsync(MealPlannerContext dbContext)
    {
        var shoppingItemsFaker = new Faker<ShoppingItem>()
            .RuleFor(si => si.Name, f => f.Lorem.Sentence().ClampLength(max: 20))
            .RuleFor(si => si.Measurement, f => f.Lorem.Sentence().ClampLength(max: 20).OrNull(f))
            .RuleFor(si => si.Price, f => f.Finance.Amount(0M, 100M).OrNull(f))
            .RuleFor(si => si.Quantity, f => f.Random.Int(1, 10).OrNull(f))
            .RuleFor(si => si.IsChecked, f => f.Random.Bool())
            .RuleFor(si => si.IsLocked, f => f.Random.Bool())
            .RuleFor(si => si.IsGenerated, _ => false)
            .RuleFor(si => si.ApplicationUserId, _ => 1);

        _shoppingItems.Add(shoppingItemsFaker.Generate());
        await dbContext.ShoppingItem.AddRangeAsync(_shoppingItems);
    }

    private Image CreateTestImage(string fileName, ManageableEntity manageableEntity, int manageableEntityId)
    {
        var randomFileName = Path.GetRandomFileName();
        var sourceFileName = Path.Combine(TestImageProcessingOptions.ImageAssetsPath, fileName);
        var destinationFileName = Path.Combine(TestImageProcessingOptions.ImageStoragePath, randomFileName);
        File.Copy(sourceFileName, destinationFileName);

        var testImage = new Image
        {
            StorageFileName = randomFileName,
            DisplayFileName = fileName,
            ImagePath = destinationFileName,
            ImageUrl = $"{TestImageProcessingOptions.ImageBaseUrl}{randomFileName}",
            ManageableEntity = manageableEntity,
            ManageableEntityId = manageableEntityId
        };

        return testImage;
    }

    public ApplicationUser GetTestUser(int index = 0) => _users[index];

    public string GetTestUserPassword(int index = 0) => _userPasswords[index];

    public Recipe GetTestRecipe(int index = 0) => _recipes[index];

    public Meal GetTestMeal(int index = 0) => _meals[index];

    public ShoppingItem GetTestShoppingItem(int index = 0) => _shoppingItems[index];

    public void Dispose()
    {
        var dirInfo = new DirectoryInfo(TestImageProcessingOptions.ImageStoragePath);

        foreach (var file in dirInfo.EnumerateFiles())
        {
            file.Delete();
        }

        _scope.Dispose();
    }
}