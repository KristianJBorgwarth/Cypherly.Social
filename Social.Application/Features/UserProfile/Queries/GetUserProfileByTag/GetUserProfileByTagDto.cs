using System.Text.Json.Serialization;
using Social.Domain.Dtos;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public sealed record GetUserProfileByTagDto
{
    public string Username { get; init; }
    public string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public Guid? AvatarKey { get; init; }
    public FriendshipStatusDto FriendshipStatus { get; init; }

    [JsonConstructor]
    private GetUserProfileByTagDto(
        string username, 
        string userTag, 
        string? displayName, 
        Guid? avatarKey,
        FriendshipStatusDto friendshipStatus)
    {
        Username = username;
        UserTag = userTag;
        DisplayName = displayName;
        AvatarKey = avatarKey;
        FriendshipStatus = friendshipStatus;
    }

    public static GetUserProfileByTagDto MapFrom(Domain.Aggregates.UserProfile friendProfile,  FriendshipStatusDto friendshipStatusDto)
    {
        return new GetUserProfileByTagDto(
            friendProfile.Username,
            friendProfile.UserTag.Tag,
            friendProfile.DisplayName,
            friendProfile.Avatar?.FileKey,
            friendshipStatusDto
        );
    }
}
