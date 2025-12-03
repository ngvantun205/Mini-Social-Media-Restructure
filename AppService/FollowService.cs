using Microsoft.AspNetCore.SignalR;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.AppService {
    public class FollowService : IFollowService {
        private readonly IFollowRepository _followRepository;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly IUserRepository _userRepository;
        public FollowService(IFollowRepository followRepository, INotificationsRepository notificationsRepository, IHubContext<NotificationsHub> hubContext, IUserRepository userRepository) {
            _followRepository = followRepository;
            _notificationsRepository = notificationsRepository;
            _hubContext = hubContext;
            _userRepository = userRepository;
        }
        public async Task<FollowDto> Follow(FollowInputModel inputModel, int followerId) {
            var follow = new Follow() {
                FollowerId = followerId,
                FolloweeId = inputModel.FolloweeId,
                CreatedAt = DateTime.UtcNow,
            };
            await _followRepository.AddAsync(follow);
            var result = await _userRepository.AddFollowAsync(inputModel.FolloweeId, followerId);
            if (!result)
                return new FollowDto() { ErrorMessage = "Cannot follow this user" };


            var noti = new Notifications() {
                ActorId = followerId,
                ReceiverId = inputModel.FolloweeId,
                EntityId = followerId,
                Type = "Follow",
                Content = "followed you.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _notificationsRepository.AddAsync(noti);
            await _hubContext.Clients.User(inputModel.FolloweeId.ToString())
                .SendAsync("ReceiveNotification", new {
                    content = noti.Content,
                    type = noti.Type,
                    postId = followerId,
                    message = $"Some one has just followed you"
                });
            return new FollowDto();

        }
        public async Task<FollowDto> Unfollow(FollowInputModel inputModel, int followerId) {
            var follow = await _followRepository.GetFollowByUserAsync(inputModel.FolloweeId, followerId);
            if (follow == null)
                return new FollowDto { ErrorMessage = "Follow record not found" };
            Console.WriteLine("==================================================================================================================================================================================================================");
            Console.WriteLine($"Follow Id: {follow.FollowId}");

            await _userRepository.DeleteFollowAsync(inputModel.FolloweeId, followerId);
            await _followRepository.DeleteAsync(follow.FollowId);
            var noti = await _notificationsRepository.GetNotification(followerId, followerId, "Follow", followerId);

            if (noti != null)
                await _notificationsRepository.DeleteAsync(noti.NotiId);
            return new FollowDto();
        }
        public async Task<IEnumerable<FollowViewModel>> GetFollowers(int userId) {
            var followers = await _followRepository.GetFollowerAsync(userId);
            return followers.Select(f => new FollowViewModel() {
                FollowId = f.FollowId,
                User = new UserSummaryViewModel() {
                    UserId = f.FollowerId,
                    AvatarUrl = f.Follower.AvatarUrl,
                    FullName = f.Follower.FullName,
                    UserName = f.Follower.UserName
                }
            });
        }
        public async Task<IEnumerable<FollowViewModel>> GetFollowee(int userId) {
            var followees = await _followRepository.GetFolloweeAsync(userId);
            return followees.Select(f => new FollowViewModel() {
                FollowId = f.FollowId,
                User = new UserSummaryViewModel() {
                    UserId = f.FolloweeId,
                    AvatarUrl = f.Followee.AvatarUrl,
                    FullName = f.Followee.FullName,
                    UserName = f.Followee.UserName
                }
            });
        }
    }
}
