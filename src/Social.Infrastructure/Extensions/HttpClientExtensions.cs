using Social.Application.Contracts.Clients;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Social.Infrastructure.Providers;

namespace Social.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
    internal static void AddProviders(this IServiceCollection services)
    {
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
