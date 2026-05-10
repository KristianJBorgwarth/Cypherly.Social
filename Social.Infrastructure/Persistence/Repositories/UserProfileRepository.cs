using Social.Application.Contracts;
using Social.Application.Contracts.Repositories;
using Social.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Social.Infrastructure.Persistence.Context;

namespace Social.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(SocialDbContext context) : IUserProfileRepository
{
    public async Task CreateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await context.UserProfile.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        context.UserProfile.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.UserProfile.FindAsync([id], cancellationToken);
    }

    public Task UpdateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        context.UserProfile.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<UserProfile?> GetByUserTag(string userTag, CancellationToken cancellationToken = default)
    {
        return await context.UserProfile.FirstOrDefaultAsync(x => x.UserTag.Tag == userTag, cancellationToken);
    }
}