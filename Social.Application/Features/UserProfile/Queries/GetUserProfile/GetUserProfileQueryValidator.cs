using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserProfileId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetUserProfileQuery.UserProfileId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserProfileQuery.UserProfileId)).Message);
    }
}