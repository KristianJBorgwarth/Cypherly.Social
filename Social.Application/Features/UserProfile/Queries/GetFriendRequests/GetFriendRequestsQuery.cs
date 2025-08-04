using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetFriendRequests;

public sealed record GetFriendRequestsQuery : IQuery<GetFriendRequestsDto[]>
{
    public required Guid TenantId { get; init; }
}