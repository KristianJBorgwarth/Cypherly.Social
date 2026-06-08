using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Services;
using Social.Infrastructure.Services;
using Social.Infrastructure.Settings;
using Social.Infrastructure.Storage.Store;
using Social.Infrastructure.Storage.Validation;

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
        services.AddServices();
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioSettings>(configuration.GetSection("Bucket"));
        services.Configure<HttpClientSettings>(configuration.GetSection("ApiBaseUrls"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        services.Configure<BlobStoreSettings>(configuration.GetSection("BlobStore"));
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAvatarService, AvatarService>();
        services.AddSingleton<IBlobStore, BlobStore>();
        services.AddScoped<IFileValidator, FileValidator>();
    }
}
