namespace Mini_Social_Media.IAppService {
    public interface IPostService {
        Task<CreatePostDto> CreatePost(PostInputModel model, int userId);
        Task<PostDto?> GetByIdAsync(int postId);
    }
}
