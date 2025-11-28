using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class NotificationsController : Controller {
        private readonly INotificationsService _notiService;

        public NotificationsController(INotificationsService notiService) {
            _notiService = notiService;
        }

        private int GetCurrentUserId() {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(idStr) ? 0 : int.Parse(idStr);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications() {
            var userId = GetCurrentUserId();
            var notis = await _notiService.GetUserNotifications(userId);

            var result = notis.Select(n => new NotificationsViewModel {
                NotiId = n.NotiId,
                ActorName = n.ActorName ?? "Someone",
                ActorAvatar = n.ActorAvatar ?? "/images/default-avatar.png",
                Message = n.Message,
                PostId = n.PostId,
                IsRead = n.IsRead,
                TimeAgo = CalculateTimeAgo(n.CreatedAt)
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead() {
            var userId = GetCurrentUserId();
            await _notiService.MarkAllAsReadAsync(userId);
            return Ok(new { success = true });
        }

        private string CalculateTimeAgo(DateTime date) {
            var span = DateTime.UtcNow - date;
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h";
            return $"{(int)span.TotalDays}d";
        }
    }
}