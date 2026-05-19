using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Services;
using Social.Infrastructure.Services;
using Social.Infrastructure.Settings;
using Social.Infrastructure.Store;

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
        services.AddBlobStore();
        services.AddStorage();
        services.AddServices();
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioSettings>(configuration.GetSection("Bucket"));
        services.Configure<HttpClientSettings>(configuration.GetSection("ApiBaseUrls"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        services.Configure<BlobStoreSettings>(configuration.GetSection("BlobStore"));
    }

    private static void AddBlobStore(this IServiceCollection services)
    {
        services.AddSingleton<IBlobStore, BlobStore>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAvatarService, AvatarService>();
    }
}
