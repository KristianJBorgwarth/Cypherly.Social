using Social.Application.Abstractions;

namespace Social.Application.Features.Friendships.Commands.Update.UnblockUser;

public sealed record UnblockUserCommand : ICommand
{
    public Guid TenantId { get; init; }
    public required string Tag { get; init; }
}