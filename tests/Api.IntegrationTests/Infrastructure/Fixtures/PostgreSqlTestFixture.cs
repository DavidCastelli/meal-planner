using Testcontainers.PostgreSql;

namespace Api.IntegrationTests.Infrastructure.Fixtures;

public sealed class PostgreSqlTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    public string GetConnectionString() => _postgreSqlContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        var migrationSql = await File.ReadAllTextAsync("db_migration.sql");
        await _postgreSqlContainer.ExecScriptAsync(migrationSql);
    }

    public async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}