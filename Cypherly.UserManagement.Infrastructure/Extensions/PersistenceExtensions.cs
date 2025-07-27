using System.Reflection;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Contracts.Repositories;
using Cypherly.UserManagement.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Infrastructure.Extensions;

internal static class PersistenceExtensions
{
    private const string ConnectionStringName = "UserManagementDbConnectionString";

    internal static void AddPersistence(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddDbContext<UserManagementDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringName),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(assembly.FullName);
                    sqlOptions.EnableRetryOnFailure();
                });
        });

        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
    }
}