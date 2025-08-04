using System.Windows.Input;
using Social.Application.Abstractions;
using ICommand = Social.Application.Abstractions.ICommand;

namespace Social.Application.Features.UserProfile.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required string BlockedUserTag { get; init; }
}