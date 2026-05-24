using Microsoft.AspNetCore.Http;
using Social.Application.Contracts.Services;
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
    public Stream Get(Guid avatarId, CancellationToken ct = default) => blobStore.Open(avatarId);

    public async Task<Result<Avatar>> UploadAsync(IFormFile file, Guid userId, CancellationToken ct = default)
    {
        if (!fileValidator.IsValidImageFile(file, out var errorMessage))
            return Result.Fail<Avatar>(Error.Validation($"Value '{errorMessage}' is not valid in this context"));

        var avatarId = Guid.NewGuid();

        await using var stream = file.OpenReadStream();
        await blobStore.PutAsync(avatarId, stream, ct);

        return Result.Ok(new Avatar(
            userProfileId: userId,
            avatarId: avatarId,
            ContentType: file.ContentType,
            etag: ETag.Generate()));
    }

    public void Delete(Guid avatarId, CancellationToken ct = default)
    {
        blobStore.Delete(avatarId);
    }
}
