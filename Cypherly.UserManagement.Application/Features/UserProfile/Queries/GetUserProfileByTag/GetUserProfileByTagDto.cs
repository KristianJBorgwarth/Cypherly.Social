using System.Text.Json.Serialization;
using Cypherly.UserManagement.Domain.Dtos;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public sealed record GetUserProfileByTagDto
{
    public string Username { get; init; }
    public string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public FriendshipStatusDto FriendshipStatus { get; init; }

    [JsonConstructor]
    private GetUserProfileByTagDto(string username, string userTag, string? displayName, string? profilePictureUrl, FriendshipStatusDto friendshipStatus)
    {
        Username = username;
        UserTag = userTag;
        DisplayName = displayName;
        ProfilePictureUrl = profilePictureUrl;
        FriendshipStatus = friendshipStatus;
    }

    public static GetUserProfileByTagDto MapFrom(Domain.Aggregates.UserProfile friendProfile, string? presignedUrl, FriendshipStatusDto friendshipStatusDto)
    {
        return new GetUserProfileByTagDto(
            friendProfile.Username,
            friendProfile.UserTag.Tag,
            friendProfile.DisplayName,
            presignedUrl,
            friendshipStatusDto
        );
    }
}