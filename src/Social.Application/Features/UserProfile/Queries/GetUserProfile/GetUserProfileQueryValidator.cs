using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage($"Value '{nameof(GetUserProfileQuery.TenantId)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(GetUserProfileQuery.TenantId)} ");
    }
}
