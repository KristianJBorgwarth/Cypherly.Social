namespace Social.API.Requests.Command;

public sealed record TogglePrivacyRequest
{
    public bool IsPrivate { get; init; }
}