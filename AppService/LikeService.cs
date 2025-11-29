using Microsoft.AspNetCore.SignalR;

namespace Mini_Social_Media.AppService {
    public class LikeService : ILikeService {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly INotificationsRepository _notificationsRepository;
        public LikeService(ILikeRepository likeRepository, IPostRepository postRepository, IHubContext<NotificationsHub> hubContext, INotificationsRepository notificationsRepository) {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _hubContext = hubContext;
            _notificationsRepository = notificationsRepository;
        }
        public async Task<LikeDto> ToggleLikeAsync(LikeInputModel likeInputModel, int userId) {
            var post = await _postRepository.GetByIdAsync(likeInputModel.PostId);
            if (post != null) {
                if (!await _likeRepository.IsLikedByCurrentUser(likeInputModel.PostId, userId)) {
                    post.Likes.Add(new Like() { UserId = userId, PostId = likeInputModel.PostId, CreatedAt = DateTime.UtcNow });
                    await _postRepository.LikePostAsync(likeInputModel.PostId);
                    var receiverId = post.UserId;
                    if (receiverId != userId) {
                        var noti = new Notifications() {
                            ActorId = userId,
                            ReceiverId = receiverId,
                            EntityId = post.PostId,
                            Type = "Like",
                            Content = "liked your post.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                        };
                        await _notificationsRepository.AddAsync(noti);

                        await _hubContext.Clients.User(receiverId.ToString())
                        .SendAsync("ReceiveNotification", new {
                            content = noti.Content,
                            type = noti.Type,
                            postId = noti.EntityId,
                            message = $"Some one has just liked post your post."
                        });
                    }
                    return new LikeDto();
                }
                else {
                    await _likeRepository.DeleteByPostIdAndUserIdAsync(likeInputModel.PostId, userId);
                    await _postRepository.UnLikePostAsync(likeInputModel.PostId);
                    var noti = await _notificationsRepository.GetNotification(userId, post.UserId, "Like", post.PostId);
                    if(noti != null)
                    await _notificationsRepository.DeleteAsync(noti.NotiId);
                    return new LikeDto();
                }
            }
            else
                return new LikeDto() { ErrorMessage = "Post was deleted or doesn't exist" };
        }
    }
}
