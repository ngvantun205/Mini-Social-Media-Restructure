using Microsoft.AspNetCore.SignalR;

namespace Mini_Social_Media.AppService {
    public class NotificationsService : INotificationsService {
        private readonly INotificationsRepository _notiRepo;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationsService(INotificationsRepository notiRepo, IHubContext<NotificationsHub> hubContext) {
            _notiRepo = notiRepo;
            _hubContext = hubContext;
        }
        private string CalculateTimeAgo(DateTime date) {
            var span = DateTime.UtcNow - date;
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h";
            return $"{(int)span.TotalDays}d";
        }
        public async Task CreateNotification(int senderId, int receiverId, string type, int entityId, string message) {
            if (senderId == receiverId)
                return;

            var noti = new Notifications {
                ActorId = senderId,
                ReceiverId = receiverId,
                Type = type,
                EntityId = entityId,
                Content = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            var notiViewModel = new NotificationsViewModel {
                NotiId = noti.NotiId,
                ActorName = "Someone", 
                ActorAvatar = "/images/avatar.png", 
                Message = message,
                PostId = entityId,
                IsRead = false,
                TimeAgo = "Just now"
            };
            await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveNotification", notiViewModel);
            await _notiRepo.AddAsync(noti);
        }

        public async Task<IEnumerable<NotificationsViewModel>> GetUserNotifications(int userId) {
            var notis = await _notiRepo.GetByReceiverIdAsync(userId);
            return notis.Select(n => new NotificationsViewModel {
                NotiId = n.NotiId,
                ActorName = n.Actor?.UserName ?? "Someone",
                ActorAvatar = n.Actor?.AvatarUrl ?? "/images/default-avatar.png",
                Message = n.Content,
                PostId = n.EntityId,
                IsRead = n.IsRead,
                TimeAgo = CalculateTimeAgo(n.CreatedAt),
                Type = n.Type,
            }).ToList();
        }

        public async Task MarkAllAsReadAsync(int userId) {
            await _notiRepo.MarkAllAsReadAsync(userId);
        }
        
    }
}
