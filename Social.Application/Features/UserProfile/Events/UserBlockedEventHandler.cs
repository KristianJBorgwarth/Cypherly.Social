using Cypherly.Application.Abstractions;
using Social.Application.Contracts;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Events;

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