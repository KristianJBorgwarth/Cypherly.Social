using Microsoft.AspNetCore.Http;
using Social.Application.Contracts.Services;
using Social.Domain.Common;
using Social.Infrastructure.Storage.Store;
using Social.Infrastructure.Storage.Validation;

namespace Social.Infrastructure.Services;

internal sealed class AvatarService(
    IBlobStore blobStore,
    IFileValidator fileValidator)
    : IAvatarService
{
    public Stream Get(Guid avatarId) => blobStore.Open(avatarId);

    public async Task<Result<AvatarStream>> UploadAsync(IFormFile file, Guid avatarId, CancellationToken ct = default)
    {
        if (!fileValidator.IsValidImageFile(file, out var errorMessage))
            return Result.Fail<AvatarStream>(Error.Validation($"Value '{errorMessage}' is not valid in this context"));

        await using var stream = file.OpenReadStream();
        await blobStore.PutAsync(avatarId, stream, ct);

        return Result.Ok(new AvatarStream(stream, file.ContentType));
    }

    public void Delete(Guid avatarId) => blobStore.Delete(avatarId);
}
