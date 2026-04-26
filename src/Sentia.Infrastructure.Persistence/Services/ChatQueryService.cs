using Dapper;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Chats.Dtos;

namespace Sentia.Infrastructure.Persistence.Services;

public class ChatQueryService(ISqlConnectionFactory sqlConnectionFactory) : IChatQueryService
{
    public async Task<List<ChatSummaryDto>> GetUserChatsAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
                c.Id AS ChatId,
                other_cp.UserId AS OtherParticipantId,
                u.UserName AS OtherParticipantUsername,
                c.LastMessageAt AS LastMessageAt,
                lm.Content AS LastMessageContent,
                lm.SenderId AS LastMessageSenderId,
                (
                    SELECT COUNT(*)
                    FROM Messages m
                    WHERE m.ChatId = c.Id
                      AND m.SenderId != @UserId
                      AND (
                          crs.LastReadMessageId IS NULL 
                          OR read_msg.Id IS NULL 
                          OR m.CreatedAt > read_msg.CreatedAt
                      )
                ) AS UnreadCount,
                other_crs.LastReadMessageId AS OtherParticipantLastReadMessageId
            FROM Chats c
            -- Find 'me' in the chat
            INNER JOIN ChatParticipants my_cp 
                ON c.Id = my_cp.ChatId AND my_cp.UserId = @UserId
            -- Find the 'other person' in the chat
            INNER JOIN ChatParticipants other_cp 
                ON c.Id = other_cp.ChatId AND other_cp.UserId != @UserId
            -- Safely join Identity users to get their name
            INNER JOIN AspNetUsers u 
                ON other_cp.UserId = u.Id
            -- Left join my read status
            LEFT JOIN ChatReadStatus crs 
                ON c.Id = crs.ChatId AND crs.UserId = @UserId
            LEFT JOIN Messages read_msg 
                ON crs.LastReadMessageId = read_msg.Id
            LEFT JOIN ChatReadStatus other_crs 
                ON c.Id = other_crs.ChatId AND other_crs.UserId = other_cp.UserId
            -- Outer Apply gets the single most recent message instantly
            OUTER APPLY (
                SELECT TOP 1 m.Content, m.SenderId
                FROM Messages m
                WHERE m.ChatId = c.Id
                ORDER BY m.CreatedAt DESC
            ) lm
            WHERE c.Type = 1 
            ORDER BY c.LastMessageAt DESC;";

        using var connection = sqlConnectionFactory.CreateConnection();

        // Dapper automatically maps the SQL columns to the ChatSummaryDto properties
        var results = await connection.QueryAsync<ChatSummaryDto>(
            sql,
            new { UserId = userId });

        return results.AsList();
    }
}