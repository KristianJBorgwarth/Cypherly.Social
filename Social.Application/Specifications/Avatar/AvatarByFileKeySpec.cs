using Social.Application.Abstractions;
using Social.Domain.Entities;

internal sealed class AvatarByFileKeySpec(Guid fileKey) : Specification<Avatar>(a => a.FileKey == fileKey);
