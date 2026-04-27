using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Infrastructure;

public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options) { }

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ChatParticipant> ChatParticipants => Set<ChatParticipant>();
    public DbSet<ChatReadStatus> ChatReadStatus => Set<ChatReadStatus>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatParticipant>()
            .HasKey(cp => new { cp.ChatId, cp.UserId });

        modelBuilder.Entity<ChatReadStatus>()
            .HasKey(crs => new { crs.ChatId, crs.UserId });

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Message>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Chat>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<ChatParticipant>()
            .HasOne(cp => cp.Chat)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChatId);

        modelBuilder.Entity<ChatReadStatus>()
            .HasOne(crs => crs.LastReadMessage)
            .WithMany()
            .HasForeignKey(crs => crs.LastReadMessageId)
            .IsRequired(false);

        modelBuilder.Entity<ChatReadStatus>()
            .HasOne(crs => crs.Chat)
            .WithMany()
            .HasForeignKey(crs => crs.ChatId);
    }

    public static TestApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
