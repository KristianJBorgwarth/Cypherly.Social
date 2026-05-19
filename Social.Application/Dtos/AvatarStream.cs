namespace Social.Application.Dtos;

public sealed record AvatarStream
{
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
}
