using Social.Domain.Aggregates;
using Social.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Social.Infrastructure.Persistence.Outbox;

namespace Social.Infrastructure.Persistence.Context;

public class SocialDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessage { get; init; }
    public DbSet<UserProfile> UserProfile { get; init; }
    public DbSet<Friendship> Friendship { get; init; }
    public DbSet<BlockedUser> BlockedUser { get; init; }
    public DbSet<Avatar> Avatar { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}