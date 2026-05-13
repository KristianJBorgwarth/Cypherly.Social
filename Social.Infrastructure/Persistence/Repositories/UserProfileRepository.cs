using Social.Application.Contracts;
using Social.Application.Contracts.Repositories;
using Social.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Social.Application.Abstractions;
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

    public async Task<UserProfile?> GetSingleAsync(ISpecification<UserProfile> spec, CancellationToken cancellationToken = default)
    {
        var q = context.UserProfile.Where(spec.Criteria);
        
        q = spec.Includes.Aggregate(q, (current, include) => current.Include(include));
        
        return await q.FirstOrDefaultAsync(cancellationToken);
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