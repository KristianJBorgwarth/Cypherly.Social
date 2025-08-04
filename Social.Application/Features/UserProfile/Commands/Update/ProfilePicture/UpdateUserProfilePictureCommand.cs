using Microsoft.AspNetCore.Http;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public sealed record UpdateUserProfilePictureCommand : ICommand<UpdateUserProfilePictureDto>
{
    public required Guid TenantId { get; init; }
    public required IFormFile NewProfilePicture { get; init; }
}