using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Route("api/LiveStream/{livestreamId}")]
    [ApiController]
    public class LiveChatController : ControllerBase // Dùng ControllerBase cho API sạch hơn
    {
        private readonly ILiveChatMessageService _livechatService;
        private readonly IHubContext<LiveStreamHub> _hubContext; // Thêm cái này

        public LiveChatController(ILiveChatMessageService livechatService, IHubContext<LiveStreamHub> hubContext) {
            _livechatService = livechatService;
            _hubContext = hubContext;
        }

        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }

        // 1. Gửi tin nhắn
        // URL: POST /api/LiveStream/{id}/chat
        [HttpPost("chat")]
        public async Task<IActionResult> SendMessage(int livestreamId, [FromBody] CreateMessageRequest request) {
            var userId = GetCurrentUserId();

            // 1. Lưu vào Database
            // Hàm này nên trả về ViewModel đầy đủ (Avatar, Tên, Nội dung, Thời gian)
            var messageViewModel = await _livechatService.SendMessageAsync(userId, livestreamId, request.Content);

            if (messageViewModel == null)
                return BadRequest("Cannot send message");

            // 2. BẮN SIGNALR CHO CLIENT KHÁC (Quan trọng)
            // Gửi tới group "live_{id}" mà client đã join
            await _hubContext.Clients.Group($"live_{livestreamId}")
                             .SendAsync("ReceiveLiveMessage", messageViewModel);

            return Ok(messageViewModel);
        }

        // 2. Lấy lịch sử tin nhắn
        // URL: GET /api/LiveStream/{id}/messages
        [HttpGet("messages")]
        public async Task<IActionResult> GetLiveChatMessages(int livestreamId) {
            var messages = await _livechatService.GetMessagesAsync(livestreamId);
            return Ok(messages);
        }
    }

    // Class DTO để nhận dữ liệu từ Body
    public class CreateMessageRequest {
        public string Content { get; set; }
    }
}
