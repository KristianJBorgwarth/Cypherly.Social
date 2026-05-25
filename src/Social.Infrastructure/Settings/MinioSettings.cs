namespace Social.Infrastructure.Settings;

public sealed class MinioSettings
{
    public required string Host { get; init; } 
    public required string User { get; init; }
    public required string ProfilePictureBucket { get; init; } 
    public required string Password { get; init; } 
}
