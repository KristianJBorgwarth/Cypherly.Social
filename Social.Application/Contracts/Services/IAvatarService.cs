using Microsoft.AspNetCore.Http;
using Social.Domain.Common;

namespace Social.Application.Contracts.Services;

public interface IAvatarService
{
    Task<Result<AvatarStream>> UploadAsync(IFormFile file, Guid userId, CancellationToken ct = default);
    Stream Get(Guid avatarId);
    void Delete(Guid avatarId);
}

