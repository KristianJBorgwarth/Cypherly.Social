using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public sealed record GetUserProfileQuery : IQuery<GetUserProfileDto>
{
    public required Guid UserProfileId { get; init; }
    public required Guid ExlusiveConnectionId { get; init; }
}