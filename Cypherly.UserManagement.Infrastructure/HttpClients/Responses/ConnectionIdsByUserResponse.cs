namespace Cypherly.UserManagement.Infrastructure.HttpClients.Responses;

public sealed record ConnectionIdsByUserResponse
{
    public IReadOnlyCollection<Guid> ConnectionIds { get; init; } = null!;
}