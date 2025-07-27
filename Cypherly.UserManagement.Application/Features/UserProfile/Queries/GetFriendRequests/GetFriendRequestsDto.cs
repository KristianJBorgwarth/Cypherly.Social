namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsDto
{
    public string Username { get; private init; } = null!;
    public string UserTag { get; private init; } = null!;
    public string? DisplayName { get; private init; }
    public string? ProfilePictureUrl { get; private init; }

    private GetFriendRequestsDto() { }

    public static GetFriendRequestsDto MapFrom(string username, string userTag, string? displayName, string? profilePictureUrl = null)
    {
        return new GetFriendRequestsDto
        {
            Username = username,
            UserTag = userTag,
            DisplayName = displayName,
            ProfilePictureUrl = profilePictureUrl
        };
    }
}