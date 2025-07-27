using Social.Domain.Aggregates;
using Social.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public class BlockedUserModelConfiguration : BaseModelConfiguration<BlockedUser>
{
    public override void Configure(EntityTypeBuilder<BlockedUser> builder)
    {
        builder.ToTable("blocked_user");

        builder.Ignore(e => e.Id);

        builder.HasKey(e => new { UserId = e.BlockingUserProfileId, BlockedUserId = e.BlockedUserProfileId });

        builder.Property(e => e.BlockingUserProfileId)
            .HasColumnName("blocked_user_profile_id")
            .IsRequired();
        
        builder.Property(e => e.BlockingUserProfileId)
            .HasColumnName("blocking_user_profile_id")
            .IsRequired();
        

        builder.HasOne<UserProfile>(c => c.BlockedUserProfile)
            .WithMany()
            .HasForeignKey(c => c.BlockedUserProfileId);
        
        base.Configure(builder);
    }
}