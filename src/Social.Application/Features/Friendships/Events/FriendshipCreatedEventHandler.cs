using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Domain.Events.Friendships;

namespace Social.Application.Features.Friendships.Events;

public class FriendshipCreatedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendRequestMessage> producer,
    ILogger<FriendshipCreatedEventHandler> logger)
    : IDomainEventHandler<FriendshipCreatedEvent>
{
    public async Task Handle(FriendshipCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userProfileRepository.GetSingleAsync(new UserProfileWithAvatarAndFriendsSpec(notification.IntiateeId), cancellationToken);
            if (user is null)
            {
                logger.LogCritical("User not found with {ID}", notification.IntiateeId);
                throw new InvalidOperationException("Could not find user with");
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsSingleTenant(user.Id, cancellationToken);
            if (connectionIds.Count <= 0) return;

            var friend = await userProfileRepository.GetSingleAsync(new UserProfileWithAvatarAndFriendsSpec(notification.IntiateeId), cancellationToken);

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


            var message = new FriendRequestMessage()
            {
                CorrelationId = Guid.NewGuid(),
                UserId = user.Id,
                FriendUsername = friend.Username,
                FriendTag = friend.UserTag.Tag,
                FriendRequestDate = friendship.Created,
                IsSeen = friendship.IsSeen,
                FriendDisplayName = friend.DisplayName,
                AvatarKey = friend.Avatar?.FileKey,
                ConnectionIds = connectionIds,
            };
            
            await producer.PublishMessageAsync(message, cancellationToken);

        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while attempting to handle FriendshipCreatedEvent for UserProfile with {ID}", notification.IntiateeId);
        }
    }
}
