using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Domain.Entities;

namespace Social.Infrastructure.Persistence.ModelConfigurations;

public sealed class AvatarModelConfiguration : BaseModelConfiguration<Avatar>
{
    public override void Configure(EntityTypeBuilder<Avatar> builder)
    {
        builder.ToTable("avatar");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.OwnsOne(a => a.Etag, etag =>
        {
            etag.Property(e => e.Value)
                .HasColumnName("e_tag")
                .IsRequired();
        });

        builder.Property(e => e.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();

        builder.Property(e => e.ConentType)
            .HasColumnName("content_type")
            .IsRequired();

        builder.Property(e => e.FileKey)
            .HasColumnName("file_key")
            .IsRequired();
    }
}
