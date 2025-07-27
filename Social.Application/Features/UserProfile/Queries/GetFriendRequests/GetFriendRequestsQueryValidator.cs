using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsQueryValidator : AbstractValidator<GetFriendRequestsQuery>
{
    public GetFriendRequestsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendRequestsQuery.UserId)).Message);
    }
}