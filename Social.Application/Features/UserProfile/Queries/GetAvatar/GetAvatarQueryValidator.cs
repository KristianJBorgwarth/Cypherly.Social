using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed class GetAvatarQueryValidator : AbstractValidator<GetAvatarQuery>
{
    public GetAvatarQueryValidator()
    {
        RuleFor(x => x.AvatarId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsRequired(nameof(GetAvatarQuery.AvatarId)).Message);
    }
}
