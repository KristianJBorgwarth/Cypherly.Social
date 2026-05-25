using Social.Application.Abstractions;
using Social.Domain.Entities;

internal sealed class AvatarSpec(Guid avatarId) : Specification<Avatar>(a => a.Id == avatarId);

