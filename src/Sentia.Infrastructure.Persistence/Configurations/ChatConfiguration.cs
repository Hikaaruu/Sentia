using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentia.Domain.Entities;

namespace Sentia.Infrastructure.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).UseIdentityColumn();

        builder.Property(c => c.Type).IsRequired();

        builder.Property(c => c.Title).HasMaxLength(255);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.LastMessageAt)
            .HasDatabaseName("IX_Chats_LastMessageAt");
    }
}