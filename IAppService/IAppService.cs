namespace Mini_Social_Media.IAppService {
    public interface IPostService {
        Task<CreatePostDto> CreatePost(PostInputModel model, int userId);
        Task<PostDto?> GetByIdAsync(int postId);
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
}
