using Microsoft.Extensions.Logging;

namespace Sentia.Infrastructure.RealTime.Services;

public class PresenceTracker
{
    private readonly Dictionary<string, HashSet<string>> _onlineUsers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _lock = new();

    private readonly ILogger<PresenceTracker> _logger;

    public PresenceTracker(ILogger<PresenceTracker> logger)
    {
        _logger = logger;
    }

    public bool UserConnected(string userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_onlineUsers.TryGetValue(userId, out HashSet<string>? connections))
            {
                connections = [];
                _onlineUsers[userId] = connections;
            }

            connections.Add(connectionId);

            return connections.Count == 1;
        }
    }

    public bool UserDisconnected(string userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_onlineUsers.TryGetValue(userId, out var connections))
            {
                return false;
            }

            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                _onlineUsers.Remove(userId);
                return true;
            }

            return false;
        }
    }

    public IEnumerable<string> GetOnlineUsers()
    {
        lock (_lock)
        {
            return _onlineUsers.Keys.ToArray();
        }
    }
}