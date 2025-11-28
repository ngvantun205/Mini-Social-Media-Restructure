using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mini_Social_Media.Hubs {
    [Authorize]
    public class ChatHub : Hub {
        public override async Task OnConnectedAsync() {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await base.OnConnectedAsync();
        }
    }
}