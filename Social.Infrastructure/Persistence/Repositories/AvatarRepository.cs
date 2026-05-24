using Microsoft.EntityFrameworkCore;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Domain.Entities;
using Social.Infrastructure.Persistence.Context;

internal sealed class AvatarRepository(SocialDbContext ctx) : IAvatarRepository
{
    public async Task CreateAsync(Avatar entity, CancellationToken ct = default)
    {
        await ctx.Avatar.AddAsync(entity, ct);
    }

    public Task DeleteAsync(Avatar entity, CancellationToken cancellationToken = default)
    {
        ctx.Avatar.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<Avatar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Avatar?> GetSingleAsync(ISpecification<Avatar> spec, CancellationToken cancellationToken = default)
    {
        var q = ctx.Avatar.Where(spec.Criteria);
        q = spec.Includes.Aggregate(q, (current, include) => current.Include(include));
        return q.FirstOrDefaultAsync(cancellationToken);
    }

    public Task UpdateAsync(Avatar entity, CancellationToken cancellationToken = default)
    {
        ctx.Avatar.Update(entity);
        return Task.CompletedTask;
    }
}
