using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider)
    : ILimitedQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async ValueTask<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery q, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(q.TenantId), ct);
        if (userProfile is null)
        {
            return Result.Fail<List<GetFriendsDto>>(Error.NotFound<Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
        }

        var friends = userProfile.GetFriends();

        if (friends.Count is 0) return Result.Ok(new List<GetFriendsDto>());

        var connectionIds = await connectionIdProvider.GetConnectionIdsMultipleTenants([.. friends.Select(f => f.Id)], ct);

        var friendDtos = friends.Select(f => GetFriendsDto.MapFrom(f, connectionIds.GetValueOrDefault(f.Id) ?? [])).ToList();

        return Result.Ok(friendDtos);
    }
}
