using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentia.Domain.Entities;

namespace Sentia.Infrastructure.Persistence.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.ToTable("ChatParticipants");

        builder.HasKey(cp => new { cp.ChatId, cp.UserId });

        builder.Property(cp => cp.UserId).HasMaxLength(450);

        builder.Property(cp => cp.JoinedAt).IsRequired();

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(cp => cp.UserId)
            .HasDatabaseName("IX_ChatParticipants_UserId");
    }
}