using System.Reflection;
using Cypherly.UserManagement.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddSettings(configuration);
        services.AddMassTransitRabbitMq();
        services.AddPersistence(configuration, assembly);
        services.AddProviders();
        services.AddOutboxProcessingJob(assembly);
        services.AddStorage();
        return services;
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioSettings>(configuration.GetSection("MinioSettings"));
        services.Configure<HttpClientSettings>(configuration.GetSection("ApiBaseUrls"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
    }
}