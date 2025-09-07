namespace Social.Application.Contracts.Clients;

public interface IConnectionIdProvider
{
    Task<IReadOnlyCollection<Guid>> GetConnectionIdsSingleTenant(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, List<Guid>>> GetConnectionIdsMultipleTenants(Guid[] tenantIds, CancellationToken cancellationToken = default);
}