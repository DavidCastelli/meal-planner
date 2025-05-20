using Api.Domain.Meals;
using Api.Domain.Tags;
using Api.Infrastructure;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Api.IntegrationTests.Infrastructure;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private IConfiguration _testConfiguration = null!;

    public TestWebAppFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var testConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .AddUserSecrets<TestWebAppFactory>()
                .Build();

            config.AddConfiguration(testConfig);

            _testConfiguration = config.Build();
        });

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<MealPlannerContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.MapEnum<TagType>();
            dataSourceBuilder.MapEnum<Schedule>();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<MealPlannerContext>(options =>
            {
                options.UseNpgsql(dataSource);
            });

            services.Configure<TestImageProcessingOptions>(
                _testConfiguration.GetSection(TestImageProcessingOptions.ImageProcessing));
        });
    }
}