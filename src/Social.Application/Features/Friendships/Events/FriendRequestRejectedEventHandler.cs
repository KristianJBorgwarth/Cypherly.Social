using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Events.Friendships;

namespace Social.Application.Features.Friendships.Events;

public class FriendRequestRejectedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendRequestRejectedMessage> producer,
    ILogger<FriendRequestRejectedEvent> logger)
    : IDomainEventHandler<FriendRequestRejectedEvent>
{
    public async Task Handle(FriendRequestRejectedEvent ntf, CancellationToken ct)
    {
        try
        {
            var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileSpec(ntf.DeletedFriendId), ct);
            if (userProfile is null)
            {
                logger.LogWarning("UserProfile with ID {UserId} not found", ntf.UserId);
                throw new Exception($"UserProfile with ID {ntf.UserId} not found");
            }

            var connectionIds =
                await connectionIdProvider.GetConnectionIdsSingleTenant(ntf.UserId, ct);

            var message = new FriendRequestRejectedMessage()
            {
                CorrelationId = Guid.NewGuid(),
                ConnectionIds = connectionIds,
                RejectedUserProfileTag = userProfile.UserTag.Tag,
            };
            
            await producer.PublishMessageAsync(message, ct);
            logger.LogInformation("FriendRequestRejectedMessage published for UserProfile {UserId}", ntf.UserId);
        }
        catch (Exception ex)
        {
            logger.LogCritical("Error publishing FriendRequestRejectedMessage: {Message}", ex.Message);
            throw;
        }
    }
}
