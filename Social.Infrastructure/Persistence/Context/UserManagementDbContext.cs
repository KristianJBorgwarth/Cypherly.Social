using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Outbox_OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;
using OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;

namespace Social.Infrastructure.Persistence.Context;

public class UserManagementDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Outbox_OutboxMessage> OutboxMessage { get; init; }
    public DbSet<UserProfile> UserProfile { get; init; }
    public DbSet<Friendship> Friendship { get; init; }
    public DbSet<BlockedUser> BlockedUser { get; init; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}