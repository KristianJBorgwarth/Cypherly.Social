using Social.Infrastructure.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Social.Test.Integration.Setup.Authentication;
using Testcontainers.PostgreSql;

// ReSharper disable ClassNeverInstantiated.Global

namespace Social.Test.Integration.Setup;

public sealed class IntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
{
    private readonly string StorageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestBlobStore");

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(),
                    b => b.MigrationsAssembly(typeof(TDbContext).Assembly.FullName));
            });

            // Mock out authentication and authorization for testing
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireAssertion(_ => true))
                .AddPolicy("User", policy => policy.RequireAssertion(_ => true));

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg => { cfg.AddConsumers(typeof(TProgram).Assembly); });

            SetupBlobstore(services);
        });
    }

    private void SetupBlobstore(IServiceCollection services)
    {
        Directory.CreateDirectory(StorageDirectory);

        services.RemoveAll(typeof(IConfigureOptions<BlobStoreSettings>));

        var inMemorySettings = new Dictionary<string, string>
        {
            { "BlobStore:Root",  StorageDirectory }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        services.Configure<BlobStoreSettings>(configuration.GetSection("BlobStore"));
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Adding a delay to ensure Minio is ready
        await Task.Delay(10000);
    }

    public async new Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        Directory.Delete(StorageDirectory, true);
    }
}
