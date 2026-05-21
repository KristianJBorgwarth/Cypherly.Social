using FluentValidation;

namespace Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryValidator : AbstractValidator<GetBlockedUserProfilesQuery>
{
    public GetBlockedUserProfilesQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(GetBlockedUserProfilesQuery.TenantId)} ");
    }
}
