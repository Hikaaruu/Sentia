using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentia.Domain.Entities;

namespace Sentia.Infrastructure.Persistence.Configurations;

public class ChatReadStatusConfiguration : IEntityTypeConfiguration<ChatReadStatus>
{
    public void Configure(EntityTypeBuilder<ChatReadStatus> builder)
    {
        builder.ToTable("ChatReadStatus");

        builder.HasKey(crs => new { crs.UserId, crs.ChatId });

        builder.Property(crs => crs.UserId).HasMaxLength(450);

        builder.HasOne(crs => crs.Chat)
            .WithMany()
            .HasForeignKey(crs => crs.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(crs => crs.LastReadMessage)
            .WithMany()
            .HasForeignKey(crs => crs.LastReadMessageId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(crs => crs.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(crs => crs.ChatId)
            .HasDatabaseName("IX_ChatReadStatus_ChatId");
    }
}