using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public sealed record GetFriendRequestsQuery : IQuery<GetFriendRequestsDto[]>
{
    public required Guid UserId { get; init; }
}