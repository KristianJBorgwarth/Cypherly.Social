using FluentValidation;

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public class GetFriendsQueryValidator : AbstractValidator<GetFriendsQuery>
{
    public GetFriendsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage($"Value '{nameof(GetFriendsQuery.TenantId)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(GetFriendsQuery.TenantId)} ");
    }
}
