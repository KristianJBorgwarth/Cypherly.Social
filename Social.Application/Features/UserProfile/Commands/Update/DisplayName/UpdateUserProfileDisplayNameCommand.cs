using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public sealed record UpdateUserProfileDisplayNameCommand : ICommand<UpdateUserProfileDisplayNameDto>
{
    public required Guid TenantId { get; init; }
    public required string DisplayName { get; init; }
}