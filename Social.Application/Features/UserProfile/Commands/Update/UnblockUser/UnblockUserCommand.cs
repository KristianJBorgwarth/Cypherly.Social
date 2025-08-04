using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.UnblockUser;

public sealed record UnblockUserCommand : ICommand
{
    public Guid TenantId { get; init; }
    public required string Tag { get; init; }
}