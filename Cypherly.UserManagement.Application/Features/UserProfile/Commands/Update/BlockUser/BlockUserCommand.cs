using System.Windows.Input;
using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;
using ICommand = Cypherly.UserManagement.Application.Abstractions.ICommand;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string BlockedUserTag { get; init; }
}