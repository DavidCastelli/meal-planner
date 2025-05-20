using System.Data.Common;

using Npgsql;

using Respawn;

namespace Api.IntegrationTests.Infrastructure.Fixtures;

public class CommonTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlTestFixture _postgreSqlTestFixture = new();
    private TestWebAppFactory _webAppFactory = null!;
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;

    protected TestDataSeeder TestDataSeeder { get; private set; } = null!;

    public HttpClient CreateClient()
    {
        return _webAppFactory.CreateClient();
    }

    public TestDataSeeder CreateTestDataSeeder()
    {
        return new TestDataSeeder(_webAppFactory);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public virtual async ValueTask InitializeAsync()
    {
        await _postgreSqlTestFixture.InitializeAsync();

        _webAppFactory = new TestWebAppFactory(_postgreSqlTestFixture.GetConnectionString());

        await InitializeRespawner();

        TestDataSeeder = CreateTestDataSeeder();
        await TestDataSeeder.SeedUsersAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgreSqlTestFixture.DisposeAsync();
        await _webAppFactory.DisposeAsync();
        await _dbConnection.DisposeAsync();
        TestDataSeeder.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task InitializeRespawner()
    {
        _dbConnection = new NpgsqlConnection(_postgreSqlTestFixture.GetConnectionString());
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            TablesToIgnore =
            [
                "Tag",
                "_EFMigrationsHistory",
                "AspNetRoleClaims",
                "AspNetRoles",
                "AspNetUserClaims",
                "AspNetUserLogins",
                "AspNetUserRoles",
                "AspNetUsers",
                "AspNetUserTokens"
            ],
            WithReseed = true,
            DbAdapter = DbAdapter.Postgres
        });
    }
}