using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Mini_Social_Media.Hubs {
    public class NotificationsHub : Hub {
        public override async Task OnConnectedAsync() {
            var userId = Context.UserIdentifier;

            Debug.WriteLine($"---> [SIGNALR DEBUG] User Connected: ID = {userId}, ConnectionId = {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }
    }
}
