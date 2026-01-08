using Microsoft.AspNetCore.SignalR;
using Mini_Social_Media.Data;
using Mini_Social_Media.Hubs;
using Mini_Social_Media.Models;
using Mini_Social_Media.Models.ViewModel;
using Mini_Social_Media.Repository;
using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.AppService {
    public class LiveChatMessageService : ILiveChatMessageService {
        private readonly ILiveChatMessageRepository _chatRepo;
        private readonly ILiveStreamRepository _streamRepo;
        private readonly IHubContext<LiveStreamHub> _hubContext;
        private readonly AppDbContext _context;
        private readonly IGeminiService _geminiService;

        public LiveChatMessageService(
            ILiveChatMessageRepository chatRepo,
            ILiveStreamRepository streamRepo,
            IHubContext<LiveStreamHub> hubContext,
            AppDbContext context,
            IGeminiService geminiService) {
            _chatRepo = chatRepo;
            _streamRepo = streamRepo;
            _hubContext = hubContext;
            _context = context;
            _geminiService = geminiService;
        }

        public async Task<LiveChatMessageViewModel?> SendMessageAsync(int userId, int liveStreamId, string content) {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            // 1. Kiểm tra Livestream có đang OnAir không
            var stream = await _streamRepo.GetByIdAsync(liveStreamId);
            if (stream == null || stream.Status != LiveStreamStatus.OnAir) {
                return null; // Stream đã tắt, không cho chat
            }

            // 2. KIỂM DUYỆT BẰNG GEMINI (Optional)
            // Nếu nội dung vi phạm, chặn luôn
            bool isSafe = await _geminiService.CheckPost(content);
            if (!isSafe) {
                // Có thể gửi thông báo riêng cho user bảo là "Chat vi phạm"
                return null;
            }

            // 3. Lưu vào DB
            var message = new LiveChatMessage {
                LiveStreamId = liveStreamId,
                UserId = userId,
                Content = content,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(message);

            // 4. Lấy thông tin User để gửi kèm (Avatar, Tên)
            var user = await _context.Users.FindAsync(userId);

            var viewModel = new LiveChatMessageViewModel {
                Id = message.Id,
                UserId = userId,
                UserName = user?.UserName ?? "Unknown",
                UserAvatar = user?.AvatarUrl ?? "/images/default-avatar.png",
                Content = content,
                SentAt = message.SentAt
            };

            string groupName = $"live_{liveStreamId}";

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveLiveMessage", viewModel);

            return viewModel;
        }

        public async Task<IEnumerable<LiveChatMessageViewModel>> GetMessagesAsync(int liveStreamId) {
            var messages = await _chatRepo.GetByLiveStreamId(liveStreamId);

            return messages.Select(m => new LiveChatMessageViewModel {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.UserName ?? "Unknown",
                UserAvatar = m.User?.AvatarUrl ?? "/images/default-avatar.png",
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();
        }
    }
}