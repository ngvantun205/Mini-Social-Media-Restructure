using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Social_Media.AppService;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class LiveStreamController : Controller {
        private readonly ILiveStreamService _liveStreamService;
        private readonly ILiveChatMessageService _chatService;
        private readonly string _liveKitUrl; // URL Server

        public LiveStreamController(
            ILiveStreamService liveStreamService,
            ILiveChatMessageService chatService,
            IConfiguration config) {

            _liveStreamService = liveStreamService;
            _chatService = chatService;
            _liveKitUrl = config["LiveKitSettings:Url"]; // Lấy URL từ appsettings
        }

        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }

        // 1. MÀN HÌNH TẠO LIVE (GET)
        [HttpGet]
        public IActionResult Create() {
            return View(); // Trả về form nhập Title
        }

        // 2. XỬ LÝ NÚT "GO LIVE" (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string title) {
            if (string.IsNullOrEmpty(title))
                return View();
            var userId = GetCurrentUserId();
            var userName = User.Identity?.Name ?? "Streamer"; // Lấy tạm tên user hiện tại

            // Gọi Service, nhận về Tuple (token, roomId, roomName)
            var result = await _liveStreamService.StartLiveStreamAsync(userId, title);

            // Đổ dữ liệu vào ViewModel CÓ SẴN của bạn
            var model = new LiveRoomViewModel {
                RoomId = result.RoomId,
                Title = title,
                LiveKitUrl = _liveKitUrl,
                Token = result.Token,
                IsHost = true,
                StreamerName = userName,
                StreamerAvatar = "/images/default-avatar.png" // Hoặc lấy từ User Claims nếu có
            };

            // Trả về View Room
            return View("Room", model);
        }

        // 3. NGƯỜI XEM JOIN VÀO (GET)
        [HttpGet]
        public async Task<IActionResult> Watch(int id) {
            try {
                var userId = GetCurrentUserId();

                // Viewer chỉ cần Token
                var token = await _liveStreamService.JoinLiveStreamAsync(userId, id);

                // TODO: Nên query thêm DB để lấy Title, StreamerName cho đẹp
                // var streamInfo = await _liveStreamRepo.GetByIdAsync(id);

                var model = new LiveRoomViewModel {
                    RoomId = id,
                    Title = "Live Stream", // streamInfo.Title
                    LiveKitUrl = _liveKitUrl,
                    Token = token,
                    IsHost = false,
                    StreamerName = "Streamer", // streamInfo.User.FullName
                    StreamerAvatar = "/images/default-avatar.png"
                };

                return View("Room", model);
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
        }

        // 4. API TẮT LIVE
        [HttpPost]
        public async Task<IActionResult> EndLiveStream(int roomId) {
            var userId = GetCurrentUserId();
            await _liveStreamService.EndLiveStreamAsync(userId, roomId);
            return Json(new { success = true });
        }
    }
}