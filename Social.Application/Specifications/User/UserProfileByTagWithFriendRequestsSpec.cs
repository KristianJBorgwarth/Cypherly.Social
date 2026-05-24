using Social.Application.Abstractions;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class AvatarByEtagSpec(string tag) : Specification<Avatar>(a => a.Etag.Value == tag);

