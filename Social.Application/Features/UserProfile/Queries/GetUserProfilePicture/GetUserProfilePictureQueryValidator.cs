using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureQueryValidator : AbstractValidator<GetUserProfilePictureQuery>
{
    public GetUserProfilePictureQueryValidator()
    {
        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty()
            .WithMessage($"Value '{nameof(GetUserProfilePictureQuery.ProfilePictureUrl)}' is required.");
    }
}
