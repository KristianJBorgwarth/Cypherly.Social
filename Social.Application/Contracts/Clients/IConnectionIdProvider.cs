namespace Social.Application.Contracts.Clients;

public interface IConnectionIdProvider
{
    Task<IReadOnlyCollection<Guid>> GetConnectionIdsByUser(Guid userProfileId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, List<Guid>>> GetConnectionIdsByUsers(Guid[] userProfileIds, CancellationToken cancellationToken = default);
}