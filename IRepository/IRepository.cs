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
    }
    public interface ICommentRepository : IRepository<Comment> {
    }
    public interface ILikeRepository : IRepository<Like> {
    }
    public interface IFollowRepository : IRepository<Follow> {
    }
}
