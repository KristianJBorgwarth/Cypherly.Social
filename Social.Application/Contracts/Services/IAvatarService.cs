using Microsoft.AspNetCore.Http;
using Social.Domain.Common;
using Social.Domain.Entities;

namespace Social.Application.Contracts.Services;

public interface IAvatarService
{
    Task<Result<Avatar>> UploadAsync(IFormFile file, Guid userId, CancellationToken ct = default);
    Stream Get(Guid avatarId, CancellationToken ct = default);
    void Delete(Guid avatarId, CancellationToken ct = default);
}
