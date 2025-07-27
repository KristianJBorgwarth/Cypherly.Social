using System.Text.Json.Serialization;

namespace Cypherly.UserManagement.Infrastructure.HttpClients.Responses;

public sealed record ConnectionIdsByUsersResponse
{
    public Dictionary<Guid, List<Guid>> ConnectionIds { get; private init; }

    [JsonConstructor]
    private ConnectionIdsByUsersResponse(Dictionary<Guid, List<Guid>> connectionIds)
    {
        ConnectionIds = connectionIds;
    }
}