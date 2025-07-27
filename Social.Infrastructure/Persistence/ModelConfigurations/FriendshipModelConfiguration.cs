using Social.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public class FriendshipModelConfiguration : BaseModelConfiguration<Friendship>
{
    public override void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("friendship");

        builder.Ignore(e => e.Id);

        builder.HasKey(e => new { UserId = e.UserProfileId, FriendId = e.FriendProfileId });

        builder.Property(e => e.FriendProfileId)
            .HasColumnName("friend_profile_id")
            .IsRequired();
        
        builder.Property(e => e.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
        
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(e => e.IsSeen)
            .HasColumnName("is_seen")
            .IsRequired()
            .HasDefaultValue(false);
        
        base.Configure(builder);
    }
}