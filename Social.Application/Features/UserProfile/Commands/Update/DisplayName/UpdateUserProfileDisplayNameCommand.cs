using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public sealed record UpdateUserProfileDisplayNameCommand : ICommandId<UpdateUserProfileDisplayNameDto>
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}