using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public class GetFriendsQueryValidator : AbstractValidator<GetFriendsQuery>
{
    public GetFriendsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendsQuery.TenantId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.TenantId)).Message);
    }
}