using FluentValidation;

namespace Social.Application.Features.Friendships.Queries.GetFriendRequests;

public class GetFriendRequestsQueryValidator : AbstractValidator<GetFriendRequestsQuery>
{
    public GetFriendRequestsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"Value '{nameof(GetFriendRequestsQuery.TenantId)}' is required.");
    }
}
