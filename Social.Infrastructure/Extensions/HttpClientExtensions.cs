using Social.Application.Contracts.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Social.Infrastructure.Clients;
using Social.Infrastructure.Providers;
using Social.Infrastructure.Settings;

namespace Social.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
    internal static void AddProviders(this IServiceCollection services)
    {
        services.AddHttpClient<IMinioProxyClient, MinioProxyClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            if (string.IsNullOrWhiteSpace(options.Host))
            {
                throw new InvalidOperationException("Minio host is not configured properly.");
            }
            
            client.BaseAddress = new Uri(options.Host);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetTimeoutPolicy());

        services.AddScoped<IConnectionIdProvider, ConnectionProvider>();
    }

    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static AsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(5);
    }
}