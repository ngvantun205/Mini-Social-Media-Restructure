using Microsoft.Extensions.Configuration.UserSecrets;

namespace Mini_Social_Media.IAppService {
    public interface IPostService {
        Task<CreatePostDto> CreatePost(PostInputModel model, int userId);
        Task<PostDto?> GetByIdAsync(int postId, int userId);
        Task<PostDto?> EditPostAsync(EditPostInputModel model, int userId);
        Task<bool> DeletePostAsync(int postId, int userId);
        Task<IEnumerable<PostDto>> GetPostsPagedAsync(int pageNumber, int pageSize, int userId);
    }
    public interface IUploadService {
        Task<string> UploadAsync(IFormFile file);
    }
    public interface ILikeService {
        Task<LikeDto> ToggleLikeAsync(LikeInputModel inputModel, int userId);
    }
    public interface ICommentService {
        Task<CommentDto>? AddCommentAsync(CommentInputModel model, int userId);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
        Task<CommentDto?> EditCommentAsync(EditCommentInputModel model, int userId);
        Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId);
        Task<IEnumerable<CommentDto>> GetRepliesByCommentIdAsync(int commentId);
        Task<CommentDto?> AddReplyAsync(ReplyCommentInputModel model, int userId);
    }
    public interface IUserService {
        Task<UserProfileDto?> GetUserProfileAsync(int userId, int requesterId);
        Task<MyProfileDto?> GetMyProfileAsync(int userId);
        Task<MyProfileDto> UpdateUserAvatar(IFormFile formFile, int userId);
    }
}
