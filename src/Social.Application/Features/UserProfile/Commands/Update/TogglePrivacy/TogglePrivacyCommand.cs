using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public sealed record TogglePrivacyCommand : ICommand
{
    public Guid TenantId { get; init; } 
    public bool IsPrivate { get; init; }
}