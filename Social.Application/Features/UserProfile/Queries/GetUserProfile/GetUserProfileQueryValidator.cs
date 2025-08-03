using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetUserProfileQuery.TenantId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserProfileQuery.TenantId)).Message);
    }
}