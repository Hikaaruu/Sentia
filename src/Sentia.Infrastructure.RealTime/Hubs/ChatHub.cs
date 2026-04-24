using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sentia.Infrastructure.RealTime.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // SignalR's built-in User routing via Context.UserIdentifier handles
    // all message delivery. No group management needed.
}
