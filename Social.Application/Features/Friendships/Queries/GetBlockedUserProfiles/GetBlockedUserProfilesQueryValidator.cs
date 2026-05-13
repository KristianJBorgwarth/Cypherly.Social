using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryValidator : AbstractValidator<GetBlockedUserProfilesQuery>
{
    public GetBlockedUserProfilesQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(GetBlockedUserProfilesQuery.TenantId)).Message);
    }
}