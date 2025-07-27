using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Social.Application.Contracts.Clients;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Social.Infrastructure.HttpClients.Responses;

namespace Social.Infrastructure.HttpClients.Clients;

public class ConnectionIdClient(
    HttpClient httpClient,
    ILogger<ConnectionIdClient> logger)
    : IConnectionIdProvider
{
    private const string SingleUserConnectionIdsEndpoint = "api/user/device/connectionid";
    private const string MultipleUserConnectionIdsEndpoint = "api/user/devices/connectionids";

    public async Task<IReadOnlyCollection<Guid>> GetConnectionIdsByUser(Guid userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = QueryHelpers.AddQueryString(SingleUserConnectionIdsEndpoint, "UserId", userProfileId.ToString());

            var response = await httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            if (response.StatusCode is HttpStatusCode.NoContent) return [];

            var connectionIdResponse = await response.Content.ReadFromJsonAsync<Envelope<ConnectionIdsByUserResponse>>(cancellationToken);

            if (connectionIdResponse is null)
            {
                throw new JsonException($"Failed to deserialize response for user {userProfileId}. The API returned an empty body.");
            }

            if (connectionIdResponse.ErrorMessage is not null)
            {
                throw new JsonException($"Error retrieving connection IDs for user {userProfileId}: {connectionIdResponse.ErrorMessage}");
            }

            return connectionIdResponse.Result.ConnectionIds.Count == 0 ? [] : connectionIdResponse.Result.ConnectionIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving connection IDs for user {UserProfileId}", userProfileId);
            throw;
        }
    }
    public async Task<Dictionary<Guid, List<Guid>>> GetConnectionIdsByUsers(Guid[] userProfileIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = userProfileIds.Aggregate(MultipleUserConnectionIdsEndpoint, (current, userId) => QueryHelpers.AddQueryString(current, "UserIds", userId.ToString()));

            var response = await httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            var connectionIdResponse = await response.Content.ReadFromJsonAsync<Envelope<ConnectionIdsByUsersResponse>>(cancellationToken);

            if (connectionIdResponse is null)
            {
                throw new JsonException($"Failed to deserialize response for users {string.Join(", ", userProfileIds)}. The API returned an empty body.");
            }

            return connectionIdResponse.Result.ConnectionIds.Count == 0 ? new Dictionary<Guid, List<Guid>>()
                : connectionIdResponse.Result.ConnectionIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving connection IDs for users {UserProfileIds}", string.Join(", ", userProfileIds));
            throw;
        }
    }
}