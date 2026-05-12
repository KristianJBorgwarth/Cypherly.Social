using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Specifications;

internal sealed class UserProfileSpec(Guid id) : Specification<Domain.Aggregates.UserProfile>(user => user.Id == id);