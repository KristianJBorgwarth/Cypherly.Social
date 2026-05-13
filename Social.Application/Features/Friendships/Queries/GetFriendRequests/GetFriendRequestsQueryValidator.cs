using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Queries.GetFriendRequests;

public class GetFriendRequestsQueryValidator : AbstractValidator<GetFriendRequestsQuery>
{
    public GetFriendRequestsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendRequestsQuery.TenantId)).Message);
    }
}