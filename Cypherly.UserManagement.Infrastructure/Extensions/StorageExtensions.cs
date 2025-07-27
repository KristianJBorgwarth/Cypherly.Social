using Amazon.S3;
using Social.Application.Contracts;
using Social.Application.Contracts.Services;
using Cypherly.UserManagement.Infrastructure.S3.Services;
using Cypherly.UserManagement.Infrastructure.S3.Validation;
using Cypherly.UserManagement.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Infrastructure.Extensions;

internal static class StorageExtensions
{
    internal static void AddStorage(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3>(sp =>

        {
            var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            var credentials = new Amazon.Runtime.BasicAWSCredentials(minioSettings.User, minioSettings.Password);
            return new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = minioSettings.Host,
                ForcePathStyle = true,
            });
        });
        services.AddServices();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProfilePictureService, ProfilePictureService>();
        services.AddScoped<IFileValidator, FileValidator>();
    }
}