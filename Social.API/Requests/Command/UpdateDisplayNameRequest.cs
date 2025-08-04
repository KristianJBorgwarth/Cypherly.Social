namespace Social.API.Requests.Command;

public sealed record UpdateDisplayNameRequest
{
    public required string DisplayName { get; init; }
}