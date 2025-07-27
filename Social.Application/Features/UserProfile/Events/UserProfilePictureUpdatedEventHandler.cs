using Cypherly.Application.Abstractions;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Profile;
using Social.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Events;

public class UserProfilePictureUpdatedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProfilePictureService profilePictureService,
    IProducer<ProfilePictureUpdatedMessage> producer,
    ILogger<UserProfilePictureUpdatedEventHandler> logger)
    : IDomainEventHandler<UserProfilePictureUpdatedEvent>
{
    public async Task Handle(UserProfilePictureUpdatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(notification.UserProfileId);

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

            var presignedUrl = await profilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl!);

            var message = new ProfilePictureUpdatedMessage()
            {
                CorrelationId = Guid.NewGuid(),
                UserTag = userProfile.UserTag.Tag,
                ConnectionIds = connectionIds,
                ProfilePictureUrl = presignedUrl,
            };

            logger.LogInformation("Publishing ProfilePictureUpdatedMessage for UserProfileId: {UserProfileId} with ConnectionIds: {@ConnectionIds}", notification.UserProfileId, connectionIds);
            await producer.PublishMessageAsync(message, cancellationToken);
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

        var connectionIdDictionary = await connectionIdProvider.GetConnectionIdsByUsers(ids.ToArray());

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