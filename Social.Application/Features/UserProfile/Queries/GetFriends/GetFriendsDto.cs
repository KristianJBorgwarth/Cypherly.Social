
// ReSharper disable ConvertToPrimaryConstructor
namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public sealed class GetFriendsDto
{
    public Guid Id { get; set; }
    public string Username { get; private init; }
    public string UserTag { get; private init; }
    public string? DisplayName { get; private init; }
    public string? ProfilePictureUrl { get; private init; }

    public GetFriendsDto(Domain.Aggregates.UserProfile userProfile, string? presignedUrl)
    {
        Id = userProfile.Id;
        Username = userProfile.Username;
        UserTag = userProfile.UserTag.Tag;
        DisplayName = userProfile.DisplayName;
        ProfilePictureUrl = presignedUrl;
    }
}