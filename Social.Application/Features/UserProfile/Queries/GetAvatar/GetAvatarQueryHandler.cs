using Social.Domain.Common;
using Social.Application.Abstractions;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed class GetAvatarQueryHandler(IAvatarService avatarService) : IQueryHandler<GetAvatarQuery, GetAvatarDto>
{
    public Task<Result<GetAvatarDto>> Handle(GetAvatarQuery q, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
