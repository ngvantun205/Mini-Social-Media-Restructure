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

        private string CreateLiveKitToken(string userId, string userName, string roomName, bool isHost) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_lkApiSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var videoGrants = new Dictionary<string, object>
            {
                { "room", roomName },
                { "roomJoin", true },
                { "canPublish", isHost },      
                { "canPublishData", true },     
                { "canSubscribe", true }
            };

            var header = new JwtHeader(credentials);
            var payload = new JwtPayload
            {
                { "iss", _lkApiKey },         
                { "sub", userId },           
                { "name", userName },          
                { "video", videoGrants },   
                { "exp", DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds() }, 
                { "nbf", DateTimeOffset.UtcNow.AddSeconds(-5).ToUnixTimeSeconds() } 
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public async Task<(string Token, int RoomId, string RoomName)> StartLiveStreamAsync(int userId, string title) {
            var existingStream = await _liveStreamRepo.GetCurrentStreamByUserIdAsync(userId);
            if (existingStream != null) {
                existingStream.Status = LiveStreamStatus.Ended;
                existingStream.EndedAt = DateTime.UtcNow;
                await _liveStreamRepo.UpdateAsync(existingStream);
            }

            var roomName = $"room_{userId}_{DateTime.UtcNow.Ticks}";

            var stream = new LiveStream {
                UserId = userId,
                Title = title,
                ExternalRoomId = roomName,
                Status = LiveStreamStatus.OnAir,
                StartedAt = DateTime.UtcNow,
                ThumbnailUrl = "/images/live-placeholder.jpg"
            };
            await _liveStreamRepo.AddAsync(stream);

            string displayName = userId.ToString();
            var streamInfo = await _liveStreamRepo.GetByIdAsync(stream.Id);
            if (streamInfo?.User != null)
                displayName = streamInfo.User.FullName;

            var token = CreateLiveKitToken(userId.ToString(), displayName, roomName, isHost: true);

            return (token, stream.Id, roomName);
        }

        public async Task<string> JoinLiveStreamAsync(int userId, int roomId) {
            var stream = await _liveStreamRepo.GetByIdAsync(roomId);
            if (stream == null || stream.Status != LiveStreamStatus.OnAir) {
                throw new Exception("Live stream not found or has ended.");
            }

            string displayName = userId.ToString(); 

            return CreateLiveKitToken(userId.ToString(), displayName, stream.ExternalRoomId, isHost: false);
        }

        public async Task EndLiveStreamAsync(int userId, int roomId) {
            var stream = await _liveStreamRepo.GetByIdAsync(roomId);
            if (stream != null && stream.UserId == userId) {
                stream.Status = LiveStreamStatus.Ended;
                stream.EndedAt = DateTime.UtcNow;
                await _liveStreamRepo.UpdateAsync(stream);
            }
        }
    }
}