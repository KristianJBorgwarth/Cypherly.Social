using Cypherly.Application.Abstractions;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Cypherly.UserManagement.Application.Contracts.Clients;
using Cypherly.UserManagement.Application.Contracts.Repositories;
using Cypherly.UserManagement.Application.Contracts.Services;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public sealed class FriendshipAcceptedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendshipAcceptedMessage> producer,
    IProfilePictureService profilePictureService,
    ILogger<FriendshipAcceptedEventHandler> logger)
    : IDomainEventHandler<FriendshipAcceptedEvent>
{
    public async Task Handle(FriendshipAcceptedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling friendship accepted event for user {UserProfileId} and friend {FriendTag}",
            notification.UserProfileId, notification.FriendTag);

        var acceptor = await userProfileRepository.GetByIdAsync(notification.UserProfileId);
        var instigator = await userProfileRepository.GetByUserTag(notification.FriendTag);

        if (acceptor is null || instigator is null)
        {
            logger.LogError(
                "Could not find user with id {UserProfileId} or user with tag {FriendTag}",
                notification.UserProfileId,
                notification.FriendTag);

            throw new InvalidOperationException("Could not find accepting or instigating user");
        }

        await PublishFriendshipAcceptedAsync(acceptor, instigator, cancellationToken);

        await PublishFriendshipAcceptedAsync(instigator, acceptor, cancellationToken);

        logger.LogInformation("Friendship accepted event handled for user {UserProfileId} and friend {FriendTag}",
            notification.UserProfileId, notification.FriendTag);
    }

    private async Task PublishFriendshipAcceptedAsync(
        Domain.Aggregates.UserProfile receiver,
        Domain.Aggregates.UserProfile friend,
        CancellationToken cancellationToken)
    {
        var connectionIds = await connectionIdProvider.GetConnectionIdsByUsers([receiver.Id, friend.Id], cancellationToken);

        if (connectionIds[receiver.Id].Count == 0)
            return;

        var presignedUrl = string.Empty;

        if (friend.ProfilePictureUrl is not null)
        {
            var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(friend.ProfilePictureUrl);
            if (presignedUrlResult.Success)
            {
                presignedUrl = presignedUrlResult.Value;
            }
            else
            {
                logger.LogWarning("Failed to get presigned URL for profile picture of user {UserId}", friend.Id);
            }
        }

        var message = new FriendshipAcceptedMessage()
        {
            CorrelationId = Guid.NewGuid(),
            Username = friend.Username,
            Tag = friend.UserTag.Tag ?? throw new Exception("UserTag is null for user with Id " + friend.Id),
            DisplayName = friend.DisplayName,
            ProfilePictureUrl = presignedUrl,
            RouteIds = connectionIds[receiver.Id],
            ConnectionIds = connectionIds[friend.Id]
        };

        await producer.PublishMessageAsync(message, cancellationToken);
    }
}
