using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Social.Infrastructure.Jobs;

namespace Social.Infrastructure.Extensions;

internal static class JobExtensions
{
    private const string ConnectionStringName = "SocialDbConnectionString";

    internal static void AddOutboxProcessingJob(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddQuartz(configure =>
        {
            configure.SchedulerId = "AUTO";

            var jobKey = new JobKey($"{nameof(ProcessOutboxMessageJob)}-{assembly.GetName()}");

            configure.AddJob<ProcessOutboxMessageJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));

            configure.UsePersistentStore(store =>
            {
                store.UsePostgres(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString(ConnectionStringName)
                        ?? throw new InvalidOperationException($"Missing {ConnectionStringName} connection string.");
                });

                store.UseSystemTextJsonSerializer();
                store.UseClustering();
            });
        });

        services.AddQuartzHostedService();
    }
}
