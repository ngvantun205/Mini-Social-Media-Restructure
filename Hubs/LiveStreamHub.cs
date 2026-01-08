using Microsoft.AspNetCore.SignalR;

namespace Mini_Social_Media.Hubs {
    public class LiveStreamHub : Hub {
        public async Task JoinLiveGroup(string roomId) {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task LeaveLiveGroup(string roomId) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }
}