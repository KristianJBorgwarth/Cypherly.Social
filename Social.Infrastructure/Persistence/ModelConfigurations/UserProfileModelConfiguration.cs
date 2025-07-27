using Social.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public class UserProfileModelConfiguration : BaseModelConfiguration<UserProfile>
{
    public override void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profile");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(x => x.Username)
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.UserTag, y =>
        {
            y.Property(x => x.Tag)
                .HasColumnName("tag")
                .HasMaxLength(58) // 50 (max username length) + 1 (hash) + 6 (number) + 1 (letter)
                .IsRequired();
        });

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(20);

        builder.Property(x => x.ProfilePictureUrl)
            .HasColumnName("profile_picture_url");

        builder.HasMany(x => x.FriendshipsInitiated)
            .WithOne(x => x.UserProfile)
            .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FriendshipsReceived)
            .WithOne(x => x.FriendProfile)
            .HasForeignKey(x => x.FriendProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BlockedUsers)
            .WithOne()
            .HasForeignKey(x => x.BlockingUserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsPrivate)
            .HasColumnName("is_private")
            .IsRequired()
            .HasDefaultValue(false);
        
        base.Configure(builder);
    }
}