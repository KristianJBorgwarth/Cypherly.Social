
namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public sealed class GetFriendsDto
{
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public List<Guid> ConnectionIds { get; init; } = [];

    private GetFriendsDto() { }

    public static GetFriendsDto MapFrom(Cypherly.UserManagement.Domain.Aggregates.UserProfile userProfile, List<Guid> connectionIds, string? presignedUrl)
    {
        return new GetFriendsDto
        {
            Username = userProfile.Username,
            UserTag = userProfile.UserTag.Tag,
            DisplayName = userProfile.DisplayName,
            ProfilePictureUrl = presignedUrl,
            ConnectionIds = connectionIds
        };
    }
}