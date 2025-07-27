using Social.Domain.Aggregates;
using Social.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public class BlockedUserModelConfiguration : IEntityTypeConfiguration<BlockedUser>
{
    public void Configure(EntityTypeBuilder<BlockedUser> builder)
    {
        builder.ToTable("BlockedUser");

        builder.Ignore(e => e.Id);

        builder.HasKey(e => new { UserId = e.BlockingUserProfileId, BlockedUserId = e.BlockedUserProfileId });

        builder.Property(e => e.BlockingUserProfileId)
            .IsRequired();

        builder.HasOne<UserProfile>(c => c.BlockedUserProfile)
            .WithMany()
            .HasForeignKey(c => c.BlockedUserProfileId);
    }
}