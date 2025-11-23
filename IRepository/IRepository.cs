using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.IRepository {
    public interface IRepository<T> where T : class {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
    public interface IUserRepository : IRepository<User> {
        Task<bool> UserExist(string userName, string email);
        Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail);
    }
    public interface IPostRepository : IRepository<Post> {
        Task<IEnumerable<Post>> GetPostsPagedAsync(int page, int pageSize);
        Task LikePostAsync(int postId);
        Task UnLikePostAsync(int postId);
    }
    public interface ICommentRepository : IRepository<Comment> {
    }
    public interface ILikeRepository : IRepository<Like> {
        Task DeleteByPostIdAndUserIdAsync(int postId, int userId);
        Task<bool> IsLikedByCurrentUser(int postId, int userId); 
    }
    public interface IFollowRepository : IRepository<Follow> {
    }
    public interface IHashtagRepository : IRepository<Hashtag> {
        Task<bool> IsExist(string hashtagName);
        Task<Hashtag?> GetByNameAsync(string hashtagName);
    }
    public interface IPostMediaRepository : IRepository<PostMedia> {
        Task<PostMedia?> GetByUrlAsync(string url);
        Task RemoveRangeAsync(IEnumerable<PostMedia> postMedias);
    }
}
