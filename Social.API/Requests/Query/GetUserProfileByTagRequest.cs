namespace Social.API.Requests.Query;

public sealed record GetUserProfileByTagRequest
{
    public required string Tag { get; init; }
}