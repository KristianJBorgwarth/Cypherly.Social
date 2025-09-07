using Cypherly.Message.Contracts.Messages.Device;
using Cypherly.Message.Contracts.Responses.Device;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;

namespace Social.Infrastructure.Providers;

internal sealed class ConnectionProvider(
    IRequestClient<ConnectionIdMessage> connectionIdClient,
    IRequestClient<ConnectionIdsMessage> connectionIdsClient,
    ILogger<ConnectionProvider> logger) : IConnectionIdProvider
{
    public async Task<IReadOnlyCollection<Guid>> GetConnectionIdsSingleTenant(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var response = await connectionIdClient.GetResponse<ConnectionIdResponse>(new ConnectionIdMessage
        {
            CorrelationId = Guid.NewGuid(),
            TenantId = tenantId
        }, cancellationToken);
        
        if (response.Message.Error is not null)
        {
            logger.LogError("Error retrieving connection IDs for tenant {TenantId}: {Error}", tenantId, response.Message.Error);
            throw new Exception($"Error retrieving connection IDs for tenant {tenantId}: {response.Message.Error}");
        }
        
        return response.Message.ConnectionIds;
    }

    public async Task<Dictionary<Guid, List<Guid>>> GetConnectionIdsMultipleTenants(Guid[] tenantIds, CancellationToken cancellationToken = default)
    {
        var response = await connectionIdsClient.GetResponse<ConnectionIdsResponse>(new ConnectionIdsMessage()
        {
            CorrelationId = Guid.NewGuid(),
            TenantIds = tenantIds
        }, cancellationToken);
        
        if (response.Message.Error is not null)
        {
            logger.LogError("Error retrieving connection IDs for tenants {TenantIds}: {Error}", string.Join(", ", tenantIds), response.Message.Error);
            throw new Exception($"Error retrieving connection IDs for tenants {string.Join(", ", tenantIds)}: {response.Message.Error}");
        }
        
        return response.Message.ConnectionIds ?? [];
    }
}