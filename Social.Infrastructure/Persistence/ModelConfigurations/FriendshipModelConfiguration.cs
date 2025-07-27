using Cypherly.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public class FriendshipModelConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("Friendship");

        builder.Ignore(e => e.Id);

        builder.HasKey(e => new { UserId = e.UserProfileId, FriendId = e.FriendProfileId });

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.IsSeen)
            .IsRequired()
            .HasDefaultValue(false);
    }
}