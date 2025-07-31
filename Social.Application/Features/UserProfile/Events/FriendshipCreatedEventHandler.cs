using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Social.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Events;

public class FriendshipCreatedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProfilePictureService profilePictureService,
    IProducer<FriendRequestMessage> producer,
    ILogger<FriendshipCreatedEventHandler> logger)
    : IDomainEventHandler<FriendshipCreatedEvent>
{
    public async Task Handle(FriendshipCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userProfileRepository.GetByIdAsync(notification.IntiateeId);
            if (user is null)
            {
                logger.LogCritical("User not found with {ID}", notification.IntiateeId);
                throw new InvalidOperationException("Could not find user with");
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsByUser(user.Id, cancellationToken);
            if (connectionIds.Count <= 0) return;

            var friend = await userProfileRepository.GetByIdAsync(notification.InitiatorId);

            if (friend is null)
            {
                logger.LogCritical("User not found with {ID}", notification.InitiatorId);
                throw new InvalidOperationException("Could not find user with");
            }

            var friendship = friend.FriendshipsInitiated.FirstOrDefault(x => x.FriendProfileId == user.Id);

            if (friendship is null)
            {
                logger.LogCritical("Friendship not found for user {ID}", user.Id);
                throw new InvalidOperationException("Could not find friendship");
            }

            var presignedUrl = await FetchPresignedUrl(friend.ProfilePictureUrl);

            var message = new FriendRequestMessage()
            {
                CorrelationId = Guid.NewGuid(),
                UserId = user.Id,
                FriendUsername = friend.Username,
                FriendTag = friend.UserTag.Tag,
                FriendRequestDate = friendship.Created,
                IsSeen = friendship.IsSeen,
                FriendDisplayName = friend.DisplayName,
                FriendProfilePictureUrl = presignedUrl,
                ConnectionIds = connectionIds,
            };
            
            await producer.PublishMessageAsync(message, cancellationToken);

        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while attempting to handle FriendshipCreatedEvent for UserProfile with {ID}", notification.IntiateeId);
        }
    }

    private async Task<string?> FetchPresignedUrl(string? profilePictureUrl)
    {
        if (string.IsNullOrEmpty(profilePictureUrl)) return null;

        var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(profilePictureUrl);
        if (!presignedUrlResult.Success)
        {
            logger.LogError("Failed to fetch presigned URL for profile picture: {Error}", presignedUrlResult.Error);
            return null;
        }

        return presignedUrlResult.Value;
    }
}