using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public class GetFriendsQueryValidator : AbstractValidator<GetFriendsQuery>
{
    public GetFriendsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendsQuery.TenantId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.TenantId)).Message);
    }
}