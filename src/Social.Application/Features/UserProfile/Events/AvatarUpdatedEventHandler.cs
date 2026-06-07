using Social.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Profile;

namespace Social.Application.Features.UserProfile.Events;

public class AvatarUpdatedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<AvatarUpdatedMessage> producer,
    ILogger<AvatarUpdatedEventHandler> logger)
    : IDomainEventHandler<AvatarUpdatedEvent>
{
    public async Task Handle(AvatarUpdatedEvent notification, CancellationToken ct)
    {
        try
        {
            var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithAvatarAndFriendsSpec(notification.UserProfileId), ct);

            if (userProfile is null)
            {
                logger.LogWarning("User profile not found for ID: {UserProfileId}", notification.UserProfileId);
                throw new Exception($"User profile with ID {notification.UserProfileId} not found.");
            }

            var connectionIds = await GetConnectionIds(userProfile);

            if (connectionIds.Count == 0)
            {
                logger.LogWarning("No connection IDs found for UserProfileId: {UserProfileId}", notification.UserProfileId);
                return;
            }

            var message = new AvatarUpdatedMessage()
            {
                CorrelationId = Guid.NewGuid(),
                UserTag = userProfile.UserTag.Tag,
                ConnectionIds = connectionIds,
                AvatarKey = userProfile.Avatar!.FileKey
            };

            logger.LogInformation("Publishing ProfilePictureUpdatedMessage for UserProfileId: {UserProfileId} with ConnectionIds: {@ConnectionIds}", notification.UserProfileId, connectionIds);
            await producer.PublishMessageAsync(message, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while handling UserProfilePictureUpdatedEvent for UserProfileId: {UserProfileId}", notification.UserProfileId);
            throw;
        }
    }

    private async Task<IReadOnlyCollection<Guid>> GetConnectionIds(Domain.Aggregates.UserProfile userProfile)
    {
        var connectionIds = new List<Guid>();

        var ids = userProfile.GetFriends().Select(f => f.Id).ToList();
        ids.Add(userProfile.Id);

        var connectionIdDictionary = await connectionIdProvider.GetConnectionIdsMultipleTenants(ids.ToArray());

        foreach (var id in ids)
        {
            if (connectionIdDictionary.TryGetValue(id, out var idsList))
            {
                connectionIds.AddRange(idsList);
            }
        }

        return connectionIds;
    }
}
