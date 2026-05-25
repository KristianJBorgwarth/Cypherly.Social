namespace Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;

public sealed class AcceptFriendshipDto
{
    public required string Username { get; init; }
    public required string Tag { get; init; }
    public string? DisplayName { get; private init; }
    public Guid? AvatarKey { get; private init; }
    public IReadOnlyCollection<Guid> ConnectionIds { get; private init; }

    public static AcceptFriendshipDto MapFrom(Domain.Aggregates.UserProfile userProfile, IReadOnlyCollection<Guid>? connectionIds)
    {
        return new AcceptFriendshipDto
        {
            Username = userProfile.Username,
            Tag = userProfile.UserTag.Tag,
            DisplayName = userProfile.DisplayName,
            AvatarKey = userProfile.Avatar?.FileKey,
            ConnectionIds = connectionIds
        };
    }
}
