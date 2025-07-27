using Cypherly.Application.Abstractions;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Events;

public class FriendRequestRejectedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendRequestRejectedMessage> producer,
    ILogger<FriendRequestRejectedEvent> logger)
    : IDomainEventHandler<FriendRequestRejectedEvent>
{
    public async Task Handle(FriendRequestRejectedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(notification.DeletedFriendId);
            if (userProfile is null)
            {
                logger.LogWarning("UserProfile with ID {UserId} not found", notification.UserId);
                throw new Exception($"UserProfile with ID {notification.UserId} not found");
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsByUser(notification.UserId, cancellationToken);

            var message = new FriendRequestRejectedMessage()
            {
                CorrelationId = Guid.NewGuid(),
                ConnectionIds = connectionIds,
                RejectedUserProfileTag = userProfile.UserTag.Tag,
            };
            
            await producer.PublishMessageAsync(message, cancellationToken);
            logger.LogInformation("FriendRequestRejectedMessage published for UserProfile {UserId}", notification.UserId);
        }
        catch (Exception ex)
        {
            logger.LogCritical("Error publishing FriendRequestRejectedMessage: {Message}", ex.Message);
            throw;
        }
    }
}