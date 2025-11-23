using System.Runtime.InteropServices;

namespace Mini_Social_Media.AppService {
    public class LikeService : ILikeService {
        private ILikeRepository _likeRepository;
        private IPostRepository _postRepository;
        public LikeService(ILikeRepository likeRepository, IPostRepository postRepository) {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
        }
        public async Task<LikeDto> ToggleLikeAsync(LikeInputModel likeInputModel, int userId) {
            var post = await _postRepository.GetByIdAsync(likeInputModel.PostId);
            if (post != null) {
                if (!await _likeRepository.IsLikedByCurrentUser(likeInputModel.PostId, userId)) {
                    post.Likes.Add(new Like() { UserId = userId, PostId = likeInputModel.PostId, CreatedAt = DateTime.UtcNow });
                    await _postRepository.LikePostAsync(likeInputModel.PostId);
                    return new LikeDto();
                }
                else {
                    await _likeRepository.DeleteByPostIdAndUserIdAsync(likeInputModel.PostId, userId);
                    await _postRepository.UnLikePostAsync(likeInputModel.PostId);
                    return new LikeDto();
                }
            }
            else return new LikeDto() {ErrorMessage = "Post was deleted or doesn't exist" };
        }
    }
}
