

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Clients;
using Social.Infrastructure.Settings;

namespace Social.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddSettings(configuration);
        services.AddMassTransitRabbitMq();
        services.AddPersistence(configuration, assembly);
        services.AddProviders();
        services.AddOutboxProcessingJob(assembly);
        services.AddStorage();
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioSettings>(configuration.GetSection("Bucket"));
        services.Configure<HttpClientSettings>(configuration.GetSection("ApiBaseUrls"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
    }
}