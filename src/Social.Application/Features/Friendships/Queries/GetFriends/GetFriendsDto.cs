
// ReSharper disable ConvertToPrimaryConstructor
namespace Social.Application.Features.Friendships.Queries.GetFriends;

public sealed class GetFriendsDto
{
    public Guid Id { get; set; }
    public string Username { get; private init; }
    public string UserTag { get; private init; }
    public string? DisplayName { get; private init; }
    public Guid? AvatarKey { get; private init; }
    public IReadOnlyCollection<Guid> ConnectionIds { get; private init; }

    public GetFriendsDto(Domain.Aggregates.UserProfile userProfile, IReadOnlyCollection<Guid> connectionIds)
    {
        Id = userProfile.Id;
        Username = userProfile.Username;
        UserTag = userProfile.UserTag.Tag;
        DisplayName = userProfile.DisplayName;
        AvatarKey = userProfile.Avatar?.FileKey;
        ConnectionIds = connectionIds;
    }

    public static GetFriendsDto MapFrom(Domain.Aggregates.UserProfile userProfile, IReadOnlyCollection<Guid> connectionIds)
    {
        return new GetFriendsDto(userProfile, connectionIds);
    }
}
