using Social.Domain.Common;
using Social.Application.Abstractions;
using Social.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Domain.Entities;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed class GetAvatarQueryHandler(
    IAvatarRepository avatarRepository,
    IAvatarService avatarService,
    ILogger<GetAvatarQueryHandler> logger)
    : IQueryHandler<GetAvatarQuery, GetAvatarDto>
{
    public async ValueTask<Result<GetAvatarDto>> Handle(GetAvatarQuery q, CancellationToken ct)
    {
        var avatar = await avatarRepository.GetSingleAsync(new AvatarByFileKeySpec(q.FileKey), ct);

        if (avatar is null)
        {
            logger.LogWarning("Avatar with ID '{AvatarId}' not found", q.FileKey);
            return Result.Fail<GetAvatarDto>(Error.NotFound<Avatar>($"Avatar with ID '{q.FileKey}' not found"));
        }

        // The ETag only counts when it is the requested avatar's, not any avatar's.
        if (!string.IsNullOrWhiteSpace(q.ETag) && q.ETag == avatar.Etag.Value)
        {
            logger.LogInformation("Avatar with ETag '{ETag}' found, returning 304 Not Modified", q.ETag);
            return Result.Ok(new GetAvatarDto()
            {
                AvatarId = avatar.Id,
                IsModified = false,
                ETag = avatar.Etag.Value
            });
        }

        var stream = avatarService.Get(avatar.FileKey);

        return Result.Ok(new GetAvatarDto()
        {
            AvatarId = avatar.Id,
            Content = stream,
            ContentType = avatar.ConentType,
            IsModified = true,
            ETag = avatar.Etag.Value
        });
    }
}
