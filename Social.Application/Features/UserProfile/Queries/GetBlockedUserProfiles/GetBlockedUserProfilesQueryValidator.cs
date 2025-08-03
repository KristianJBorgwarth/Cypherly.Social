using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryValidator : AbstractValidator<GetBlockedUserProfilesQuery>
{
    public GetBlockedUserProfilesQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(GetBlockedUserProfilesQuery.TenantId)).Message);
    }
}