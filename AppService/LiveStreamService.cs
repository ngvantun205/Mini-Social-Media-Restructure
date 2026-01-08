using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Mini_Social_Media.AppService {
    public class LiveStreamService : ILiveStreamService {
        private readonly ILiveStreamRepository _liveStreamRepo;

        private readonly string _lkUrl;
        private readonly string _lkApiKey;
        private readonly string _lkApiSecret;

        public LiveStreamService(ILiveStreamRepository liveStreamRepo, IConfiguration config) {
            _liveStreamRepo = liveStreamRepo;
            _lkUrl = config["LiveKitSettings:Url"];
            _lkApiKey = config["LiveKitSettings:ApiKey"];
            _lkApiSecret = config["LiveKitSettings:ApiSecret"];
        }

        // --- HÀM TẠO TOKEN THỦ CÔNG (Thay thế cho AccessToken lỗi) ---
        private string CreateLiveKitToken(string userId, string userName, string roomName, bool isHost) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_lkApiSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 1. Cấu hình Video Grants (Quyền hạn)
            var videoGrants = new Dictionary<string, object>
            {
                { "room", roomName },
                { "roomJoin", true },
                { "canPublish", isHost },       // Host = true, Viewer = false
                { "canPublishData", true },     
                { "canSubscribe", true }
            };

            // 2. Tạo Header & Payload chuẩn JWT
            var header = new JwtHeader(credentials);
            var payload = new JwtPayload
            {
                { "iss", _lkApiKey },           // API Key
                { "sub", userId },              // User ID
                { "name", userName },           // Tên hiển thị trong phòng
                { "video", videoGrants },       // Quyền video
                { "exp", DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds() }, // Hết hạn sau 4h
                { "nbf", DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeSeconds() } // Hiệu lực ngay (trừ hao 5s)
            };

            // 3. Ký Token
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public async Task<(string Token, int RoomId, string RoomName)> StartLiveStreamAsync(int userId, string title) {
            // 1. Xử lý Logic Database (Tắt stream cũ nếu có)
            var existingStream = await _liveStreamRepo.GetCurrentStreamByUserIdAsync(userId);
            if (existingStream != null) {
                existingStream.Status = LiveStreamStatus.Ended;
                existingStream.EndedAt = DateTime.UtcNow;
                await _liveStreamRepo.UpdateAsync(existingStream);
            }

            // 2. Tạo Room Name duy nhất
            var roomName = $"room_{userId}_{DateTime.UtcNow.Ticks}";

            // 3. Lưu Stream mới vào DB
            var stream = new LiveStream {
                UserId = userId,
                Title = title,
                ExternalRoomId = roomName,
                Status = LiveStreamStatus.OnAir,
                StartedAt = DateTime.UtcNow,
                ThumbnailUrl = "/images/live-placeholder.jpg"
            };
            await _liveStreamRepo.AddAsync(stream);

            // 4. Lấy thông tin User để hiển thị tên đẹp (Optional)
            // Tạm thời dùng UserId làm tên, bạn có thể query UserRepo để lấy FullName
            string displayName = userId.ToString();
            var streamInfo = await _liveStreamRepo.GetByIdAsync(stream.Id);
            if (streamInfo?.User != null)
                displayName = streamInfo.User.FullName;

            var token = CreateLiveKitToken(userId.ToString(), displayName, roomName, isHost: true);

            // 3. Trả về Tuple (Token, Id, Name)
            return (token, stream.Id, roomName);
        }

        public async Task<string> JoinLiveStreamAsync(int userId, int roomId) {
            // 1. Kiểm tra phòng có tồn tại và đang Live không
            var stream = await _liveStreamRepo.GetByIdAsync(roomId);
            if (stream == null || stream.Status != LiveStreamStatus.OnAir) {
                throw new Exception("Live stream not found or has ended.");
            }

            // 2. Lấy tên Viewer
            string displayName = userId.ToString(); // Nên query DB lấy FullName thật

            // 3. GỌI HÀM TẠO TOKEN (Quyền Host = false -> Chỉ xem)
            return CreateLiveKitToken(userId.ToString(), displayName, stream.ExternalRoomId, isHost: false);
        }

        public async Task EndLiveStreamAsync(int userId, int roomId) {
            var stream = await _liveStreamRepo.GetByIdAsync(roomId);
            // Chỉ chủ phòng mới được tắt
            if (stream != null && stream.UserId == userId) {
                stream.Status = LiveStreamStatus.Ended;
                stream.EndedAt = DateTime.UtcNow;
                await _liveStreamRepo.UpdateAsync(stream);
            }
        }
    }
}