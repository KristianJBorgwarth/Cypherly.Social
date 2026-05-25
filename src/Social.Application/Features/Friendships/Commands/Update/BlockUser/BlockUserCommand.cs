using ICommand = Social.Application.Abstractions.ICommand;

namespace Social.Application.Features.Friendships.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required string BlockedUserTag { get; init; }
}