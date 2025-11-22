namespace Mini_Social_Media.IAppService {
    public interface IPostService {
        Task<CreatePostDto> CreatePost(PostInputModel model, int userId);
        Task<PostDto?> GetByIdAsync(int postId);
        Task<PostDto?> EditPostAsync(EditPostInputModel model, int userId);
    }
    public interface IUploadService {
        Task<string> UploadAsync(IFormFile file);
    }
}
