using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Notifications.Hubs;

[Authorize]
public class MessageHub : Hub<IMessageHubClient>
{
}
