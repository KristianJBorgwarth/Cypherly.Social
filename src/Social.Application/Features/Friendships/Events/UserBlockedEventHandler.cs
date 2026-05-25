using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Domain.Events.UserProfile;

namespace Social.Application.Features.Friendships.Events;

public class UserBlockedEventHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<UserBlockedEventHandler> logger)
    : IDomainEventHandler<UserBlockedEvent>
{
    public async Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken)
    {
        //TODO - Implement logic to notify blocked user to delete their friendship data, if we decide to cache friendship data in the future
    }
}