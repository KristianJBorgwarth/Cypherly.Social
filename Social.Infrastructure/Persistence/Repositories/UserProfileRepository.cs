using Social.Application.Contracts;
using Social.Application.Contracts.Repositories;
using Social.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Social.Infrastructure.Persistence.Context;

namespace Social.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(UserManagementDbContext context) : IUserProfileRepository
{
    public async Task CreateAsync(UserProfile entity)
    {
        await context.UserProfile.AddAsync(entity);
    }

    public Task DeleteAsync(UserProfile entity)
    {
        context.UserProfile.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await context.UserProfile.FindAsync(id);
    }

    public Task UpdateAsync(UserProfile entity)
    {
        context.UserProfile.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<UserProfile?> GetByUserTag(string userTag)
    {
        return await context.UserProfile.FirstOrDefaultAsync(x => x.UserTag.Tag == userTag);
    }
}