using System.Windows.Input;
using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;
using ICommand = Social.Application.Abstractions.ICommand;

namespace Social.Application.Features.UserProfile.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string BlockedUserTag { get; init; }
}