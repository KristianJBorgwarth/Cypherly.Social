using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public sealed record GetUserProfileByTagQuery : IQuery<GetUserProfileByTagDto>
{
    public required Guid TenantId { get; init; }
    public required string Tag { get; init; }
}