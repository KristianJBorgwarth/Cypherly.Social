using Microsoft.AspNetCore.Http;
using Social.Application.Contracts.Services;
using Social.Application.Dtos;
using Social.Domain.Common;
using Social.Domain.Entities;
using Social.Domain.ValueObjects;
using Social.Infrastructure.S3.Validation;
using Social.Infrastructure.Store;

namespace Social.Infrastructure.Services;

internal sealed class AvatarService(
    IBlobStore blobStore,
    IFileValidator fileValidator)
    : IAvatarService
{
    public AvatarStream Get(Guid avatarId, CancellationToken ct = default)
    {
        var stream = blobStore.Open(avatarId);
        return new AvatarStream() { Content = stream };
    }

    public async Task<Result<Avatar>> UploadAsync(IFormFile file, Guid userId, CancellationToken ct = default)
    {
        if (!fileValidator.IsValidImageFile(file, out var errorMessage))
            return Result.Fail<Avatar>(Errors.General.UnexpectedValue(errorMessage));

        var avatarId = Guid.NewGuid();

        await using var stream = file.OpenReadStream();
        await blobStore.PutAsync(avatarId, stream, ct);

        return Result.Ok(new Avatar(
            userProfileId: userId,
            avatarId: avatarId,
            etag: ETag.Generate()));
    }

    public void Delete(Guid avatarId, CancellationToken ct = default)
    {
        blobStore.Delete(avatarId);
    }
}
