namespace Social.API.Requests.Command;

public sealed record UpdateUserProfilePictureRequest
{
    public required IFormFile ProfilePicture { get; init; }
}