using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed record GetUserProfilePictureQuery : IQuery<GetUserProfilePictureDto>
{
    public required string ProfilePictureUrl { get; init; }
}