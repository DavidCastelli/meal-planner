using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Infrastructure;

[Collection("TestCollection")]
public abstract class TestBase : IAsyncLifetime
{
    protected CommonTestFixture Fixture { get; }
    protected HttpClient HttpClient { get; }
    protected TestDataSeeder TestDataSeeder { get; }

    protected TestBase(CommonTestFixture fixture)
    {
        Fixture = fixture;
        HttpClient = Fixture.CreateClient();
        TestDataSeeder = Fixture.CreateTestDataSeeder();
    }

    public async ValueTask InitializeAsync()
    {
        await TestDataSeeder.SeedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Fixture.ResetDatabaseAsync();
        TestDataSeeder.Dispose();
        GC.SuppressFinalize(this);
    }
}