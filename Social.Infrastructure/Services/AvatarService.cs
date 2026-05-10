using Microsoft.AspNetCore.Http;
using Social.Application.Contracts.Services;
using Social.Application.Dtos;
using Social.Domain.Common;
using Social.Infrastructure.Persistence.Context;
using Social.Infrastructure.Store;

internal sealed class AvatarService(SocialDbContext dbContext, IBlobStore blobStore) : IAvatarService
{
    public Task<Result> DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AvatarStream>> GetAsync(Guid blobId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Upload(IFormFile file, Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
