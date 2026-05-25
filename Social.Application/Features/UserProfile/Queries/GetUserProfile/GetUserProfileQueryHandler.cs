using Social.Domain.Common;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery q, CancellationToken ct)
    {
        var up = await userProfileRepository.GetSingleAsync(new UserProfileSpec(q.TenantId), ct);
        if (up is null) return Result.Fail<GetUserProfileDto>(Error.NotFound<Domain.Aggregates.UserProfile>(q.TenantId.ToString()));

        var dto = GetUserProfileDto.MapFrom(up);
        return Result.Ok(dto);
    }
}
