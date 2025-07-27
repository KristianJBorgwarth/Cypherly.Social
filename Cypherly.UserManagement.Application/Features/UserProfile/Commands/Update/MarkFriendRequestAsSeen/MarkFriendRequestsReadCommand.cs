using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsReadCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required IReadOnlyCollection<string> RequestTags { get; init; }
}