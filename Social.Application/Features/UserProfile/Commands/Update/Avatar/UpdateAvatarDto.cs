namespace Social.Application.Features.UserProfile.Commands.Update.Avatar;

public sealed record UpdateAvatarDto
{
    public required Guid FileKey { get; init; }
    public required string Etag { get; init; }
}
