using Microsoft.AspNetCore.Http;
using Social.Application.Dtos;
using Social.Domain.Common;

namespace Social.Application.Contracts.Services;

public interface IAvatarService
{
    Task<Result> Upload(IFormFile file, Guid userId, CancellationToken ct = default);
    Task<Result<AvatarStream>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid userId, CancellationToken ct = default);
}
