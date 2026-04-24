using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentia.Domain.Entities;

namespace Sentia.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasMaxLength(40)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(m => m.ChatId).IsRequired();

        builder.Property(m => m.SenderId).HasMaxLength(450).IsRequired();

        builder.Property(m => m.Content).HasMaxLength(4000).IsRequired();

        builder.Property(m => m.CreatedAt).IsRequired();

        builder.Property(m => m.SentimentLabel)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(m => new { m.ChatId, m.CreatedAt })
            .HasDatabaseName("IX_Messages_ChatId_CreatedAt");
    }
}