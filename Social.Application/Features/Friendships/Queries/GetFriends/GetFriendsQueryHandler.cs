using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository)
    : ILimitedQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async Task<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery q, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(q.TenantId), ct);
        if (userProfile is null)
        {
            return Result.Fail<List<GetFriendsDto>>(Error.NotFound<Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
        }

        var friends = userProfile.GetFriends();

        if (friends.Count is 0) return Result.Ok(new List<GetFriendsDto>());

        var friendDtos = friends.Select(GetFriendsDto.MapFrom).ToList();

        return Result.Ok(friendDtos);
    }
}
