using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryValidator : AbstractValidator<GetUserProfileByTagQuery>
{
    public GetUserProfileByTagQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(GetUserProfileByTagQuery.TenantId)} ");
        RuleFor(x => x.Tag)
            .NotNull().WithMessage($"Value '{nameof(GetUserProfileByTagQuery.Tag)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(GetUserProfileByTagQuery.Tag)} ");
    }
}
