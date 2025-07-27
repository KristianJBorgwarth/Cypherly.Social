using Social.Infrastructure.Settings;
using Cypherly.UserManagement.Test.Integration.Setup.Authentication;
using Cypherly.UserManagement.Test.Integration.Setup.Helpers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Social.API.Filters;
using Testcontainers.PostgreSql;

// ReSharper disable ClassNeverInstantiated.Global

namespace Cypherly.UserManagement.Test.Integration.Setup;

public sealed class IntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
{
    private readonly IContainer _minioBucketContainer = new ContainerBuilder()
        .WithImage("bitnami/minio:latest")
        .WithEnvironment("MINIO_ROOT_USER", "MinioRoot")
        .WithEnvironment("MINIO_ROOT_PASSWORD", "rootErinoTest?87")
        .WithExposedPort(9000)
        .WithExposedPort(9001)
        .WithPortBinding(9023, 9000)
        .WithPortBinding(9024, 9001)
        .WithCleanUp(true)
        .Build();

    private bool ShouldTestWithLazyLoadingProxies { get; set; } = true;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .Build();

    private readonly MinioBucketHandler _minioBucketHandler = new("MinioRoot", "rootErinoTest?87");


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            #region Database Extensions
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(),
                    b => b.MigrationsAssembly(typeof(TDbContext).Assembly.FullName));

                if (ShouldTestWithLazyLoadingProxies)
                {
                    options.UseLazyLoadingProxies();
                }
            });

            #endregion

            #region Auth Extensions
            // Mock out authentication and authorization for testing
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireAssertion(_ => true))
                .AddPolicy("User", policy => policy.RequireAssertion(_ => true));

            // Mock out ValidateUserIdFilter
            // Remove the existing ValidateUserIdFilter registration
            var actionFilterDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IValidateUserIdFilter));

            if (actionFilterDescriptor != null)
            {
                services.Remove(actionFilterDescriptor);
            }

            // Replace with a mock or NoOp implementation
            services.AddScoped<IValidateUserIdFilter, MockValidateUserIdIdFilter>();

            #endregion

            #region RabbitMq Extensions

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(TProgram).Assembly);
            });

            #endregion

            #region Minio Extensions


            services.RemoveAll(typeof(IConfigureOptions<MinioSettings>));

            // Add in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"S3:Host", "http://localhost:9023"},
                {"S3:ProfilePictureBucket", "bucket-name"},
                {"S3:User", "MinioRoot"},
                {"S3:Password", "rootErinoTest?87"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            services.Configure<MinioSettings>(configuration.GetSection("S3"));

            #endregion
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _minioBucketContainer.StartAsync();

        // Adding a delay to ensure Minio is ready
        await Task.Delay(10000);
        await _minioBucketHandler.CreateBucketAsync("bucket-name");
    }

    public async new Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _minioBucketContainer.StopAsync();
    }
}