namespace Social.API.Requests.Command;

public sealed record UpdateAvatarRequest
{
    public required IFormFile Avatar { get; init; }
}
