using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;
using Social.Domain.Enums;

namespace Social.Application.Features.Friendships.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<GetFriendRequestsQueryHandler> logger)
    : IQueryHandler<GetFriendRequestsQuery, GetFriendRequestsDto[]>
{
    public async Task<Result<GetFriendRequestsDto[]>> Handle(GetFriendRequestsQuery q, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendRequestsSpec(q.TenantId), ct);

        if (userProfile is null)
        {
            logger.LogCritical("UserProfile not found, UserProfileId: {UserProfileId}", q.TenantId);
            return Result.Fail<GetFriendRequestsDto[]>(Error.NotFound<Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
        }

        var friendRequests = userProfile.FriendshipsReceived
            .Where(f => f.Status == FriendshipStatus.Pending)
            .ToList();

        var friendRequestDtos =  friendRequests.Select(f => new GetFriendRequestsDto(f.UserProfile)).ToArray();

        return Result.Ok(friendRequestDtos);
    }
}
