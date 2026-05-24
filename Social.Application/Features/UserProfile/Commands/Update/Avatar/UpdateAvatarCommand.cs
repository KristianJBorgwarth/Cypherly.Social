using Microsoft.AspNetCore.Http;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.Avatar;

public sealed record UpdateAvatarCommand : ICommand<UpdateAvatarDto>
{
    public required Guid TenantId { get; init; }
    public required IFormFile NewProfilePicture { get; init; }
}
