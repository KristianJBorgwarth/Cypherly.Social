namespace Social.Application.Features.Friendships.Queries.GetFriendRequests;

public sealed class GetFriendRequestsDto
{
    public string Username { get; private init; } 
    public string UserTag { get; private init; } 
    public string? DisplayName { get; private init; }
    public Guid? AvatarKey { get; private init; }

    public GetFriendRequestsDto(Domain.Aggregates.UserProfile userProfile)
    {
        Username = userProfile.Username;
        UserTag = userProfile.UserTag.Tag;
        DisplayName = userProfile.DisplayName;
        AvatarKey = userProfile.Avatar?.FileKey;
    }
}
