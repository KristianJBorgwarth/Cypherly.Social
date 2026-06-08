using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Events.Friendships;

namespace Social.Application.Features.Friendships.Events;

public sealed class FriendshipDeletedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendshipDeletedMessage> producer,
    ILogger<FriendshipDeletedEventHandler> logger)
    : IDomainEventHandler<FriendshipDeletedEvent>
{
    public async Task Handle(FriendshipDeletedEvent ntf, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileSpec(ntf.UserProfileId), cancellationToken);
            var deletedFriendProfile = await userProfileRepository.GetSingleAsync(new UserProfileSpec(ntf.FriendProfileId), cancellationToken);
            if (userProfile is null || deletedFriendProfile is null)
            {
                logger.LogWarning("User profile or friend profile not found for user {UserProfileId} and friend {FriendProfileId}", ntf.UserProfileId, ntf.FriendProfileId);

                throw new InvalidOperationException("User profile or friend profile not found");
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsMultipleTenants([userProfile.Id, deletedFriendProfile.Id], cancellationToken);

            var message = new FriendshipDeletedMessage
            {
                CorrelationId = Guid.NewGuid(),
                UserTag = userProfile.UserTag.Tag,
                DeletedUserTag = deletedFriendProfile.UserTag.Tag,
                ConnectionIds = connectionIds.SelectMany(x => x.Value).ToList()
            };

            await producer.PublishMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling FriendshipDeletedEvent for user {UserProfileId} and friend {FriendProfileId}", ntf.UserProfileId, ntf.FriendProfileId);
            throw;
        }
    }
}
