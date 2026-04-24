using Microsoft.EntityFrameworkCore;
using Sentia.Domain.Entities;

namespace Sentia.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Chat> Chats { get; }
    DbSet<Message> Messages { get; }
    DbSet<ChatParticipant> ChatParticipants { get; }
    DbSet<ChatReadStatus> ChatReadStatus { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
