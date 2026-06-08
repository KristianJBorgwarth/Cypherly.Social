using Social.Application.Contracts.Repositories;
using Social.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Social.Application.Abstractions;
using Social.Infrastructure.Persistence.Context;

namespace Social.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(SocialDbContext context) : IUserProfileRepository
{
    public async Task CreateAsync(UserProfile entity, CancellationToken ct = default)
    {
        await context.UserProfile.AddAsync(entity, ct);
    }

    public Task DeleteAsync(UserProfile entity, CancellationToken ct = default)
    {
        context.UserProfile.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<UserProfile?> GetSingleAsync(ISpecification<UserProfile> spec, CancellationToken ct = default)
    {
        var q = context.UserProfile.Where(spec.Criteria);
        
        q = spec.Includes.Aggregate(q, (current, include) => current.Include(include));
        
        return await q.FirstOrDefaultAsync(ct);
    }

    public Task UpdateAsync(UserProfile entity, CancellationToken ct = default)
    {
        context.UserProfile.Update(entity);
        return Task.CompletedTask;
    }
}
