using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Contracts.Clients;
using Cypherly.UserManagement.Infrastructure.HttpClients;
using Cypherly.UserManagement.Infrastructure.HttpClients.Clients;
using Cypherly.UserManagement.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Cypherly.UserManagement.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
    internal static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddHttpClient<IConnectionIdProvider, ConnectionIdClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<HttpClientSettings>>().Value;

            if (string.IsNullOrWhiteSpace(options.IdentityServiceUrl))
            {
                throw new InvalidOperationException("IdentityServiceUrl is not configured properly.");
            }

            client.BaseAddress = new Uri(options.IdentityServiceUrl);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetTimeoutPolicy());

        services.AddHttpClient<IMinioProxyClient, MinioProxyClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            client.BaseAddress = new Uri(options.Host);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetTimeoutPolicy());

        return services;
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