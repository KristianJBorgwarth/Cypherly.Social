namespace Social.API.Requests.Query;

public sealed record GetUserProfilePictureRequest
{
    public required string ProfilePictureUrl { get; init; }
}