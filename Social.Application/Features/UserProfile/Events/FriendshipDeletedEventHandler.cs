﻿using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Friendship;
using Social.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Events;

public sealed class FriendshipDeletedEventHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProducer<FriendshipDeletedMessage> producer,
    ILogger<FriendshipDeletedEventHandler> logger)
    : IDomainEventHandler<FriendshipDeletedEvent>
{
    public async Task Handle(FriendshipDeletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(notification.UserProfileId);
            var deletedFriendProfile = await userProfileRepository.GetByIdAsync(notification.FriendProfileId);
            if (userProfile is null || deletedFriendProfile is null)
            {
                logger.LogWarning("User profile or friend profile not found for user {UserProfileId} and friend {FriendProfileId}", notification.UserProfileId, notification.FriendProfileId);

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
            logger.LogError(ex, "Error handling FriendshipDeletedEvent for user {UserProfileId} and friend {FriendProfileId}", notification.UserProfileId, notification.FriendProfileId);
            throw;
        }
    }
}