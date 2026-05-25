using Social.Domain.Common;
using Social.Application.Abstractions;
using Social.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Entities;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed class GetAvatarQueryHandler(
    IAvatarRepository avatarRepository,
    IAvatarService avatarService,
    ILogger<GetAvatarQueryHandler> logger)
    : IQueryHandler<GetAvatarQuery, GetAvatarDto>
{
    public async Task<Result<GetAvatarDto>> Handle(GetAvatarQuery q, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(q.ETag))
        {
            return await HandleEtagReq(q.ETag, q.FileKey, ct);
        }
        else
        {
            return await HandleReqWithoutEtag(q.FileKey, ct);
        }
    }

    private async Task<Result<GetAvatarDto>> HandleEtagReq(string etag, Guid avatarId, CancellationToken ct)
    {
        var avatar = await avatarRepository.GetSingleAsync(new AvatarByEtagSpec(etag), ct);
        if (avatar is not null)
        {
            logger.LogInformation("Avatar with ETag '{ETag}' found, returning 304 Not Modified", etag);
            return Result.Ok(new GetAvatarDto()
            {
                AvatarId = avatar.Id,
                IsModified = false,
                ETag = avatar.Etag.Value
            });
        }

        return await HandleReqWithoutEtag(avatarId, ct);
    }

    private async Task<Result<GetAvatarDto>> HandleReqWithoutEtag(Guid fileKey, CancellationToken ct)
    {
        var avatar = await avatarRepository.GetSingleAsync(new AvatarByFileKeySpec(fileKey), ct);

        if (avatar is null)
        {
            logger.LogWarning("Avatar with ID '{AvatarId}' not found", fileKey);
            return Result.Fail<GetAvatarDto>(Error.NotFound<Avatar>($"Avatar with ID '{fileKey}' not found"));
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
