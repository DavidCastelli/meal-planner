using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.Infrastructure;

[Collection("AuthorizationTestCollection")]
public abstract class TestBaseAuthorization : IAsyncLifetime
{
    protected AuthorizationTestFixture Fixture { get; }
    protected HttpClient HttpClient { get; }
    protected TestDataSeeder TestDataSeeder { get; }

    protected TestBaseAuthorization(AuthorizationTestFixture fixture)
    {
        Fixture = fixture;
        HttpClient = Fixture.CreateClient();
        TestDataSeeder = Fixture.CreateTestDataSeeder();
        HttpClient.DefaultRequestHeaders.Add("Cookie", Fixture.AuthCookie);
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