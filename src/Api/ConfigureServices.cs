using Api.Common;
using Api.Common.Interfaces;
using Api.Infrastructure;
using Api.Infrastructure.Authorization;
using Api.Infrastructure.Identity;
using Api.Infrastructure.Services;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api;

/// <summary>
/// Utility class for registering groups of related services to the dependency injection container.
/// </summary>
public static class ConfigureServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to the application to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        foreach (var type in typeof(Program).Assembly.GetTypes()
                     .Where(type => type.Name.EndsWith("Handler", StringComparison.Ordinal) && type is { IsInterface: false, IsAbstract: false }))
        {
            services.AddScoped(type);
        }

        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

        services.AddSingleton<IImageProcessingInfo, ImageProcessingInfo>();

        return services;
    }

    /// <summary>
    /// Adds services related to the infrastructure to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application config.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MealPlannerContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IAuthorizationHandler, MealAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecipeAuthorizationHandler>();

        services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddEntityFrameworkStores<MealPlannerContext>();

        services.AddSingleton<IUserContext, UserContext>();

        return services;
    }
}