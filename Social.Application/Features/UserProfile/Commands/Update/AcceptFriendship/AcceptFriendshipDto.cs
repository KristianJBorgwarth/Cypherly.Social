namespace Social.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public sealed class AcceptFriendshipDto
{
    public required string Username { get; init; }
    public required string Tag { get; init; }
    public string? DisplayName { get; private init; }
    public string? ProfilePictureUrl { get; private init; }
    public IReadOnlyCollection<Guid> ConnectionIds { get; private init; }

    public static AcceptFriendshipDto MapFrom(Cypherly.UserManagement.Domain.Aggregates.UserProfile userProfile, string? profilePictureUrl, IReadOnlyCollection<Guid>? connectionIds)
    {
        return new AcceptFriendshipDto
        {
            Username = userProfile.Username,
            Tag = userProfile.UserTag.Tag,
            DisplayName = userProfile.DisplayName,
            ProfilePictureUrl = profilePictureUrl,
            ConnectionIds = connectionIds
        };
    }
}