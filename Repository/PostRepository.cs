using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.Repository {
    public class PostRepository : IPostRepository {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<Post>> GetAllAsync() {
            return await _context.Posts.ToListAsync();
        }
        public async Task<Post?> GetByIdAsync(int id) {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Medias)
                .Include(p => p.Comments)
                .Include(p => p.Comments!)
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefaultAsync(p => p.PostId == id);
        }
        public async Task<IEnumerable<Post>> GetPostsPagedAsync(int page, int pageSize) {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Medias)
                .Include(p => p.Likes)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Post entity) {
            await _context.Posts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var post = await GetByIdAsync(id);
            if (post != null) {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Post entity) {
            var existingPost = await _context.Posts.FindAsync(entity.PostId);
            if (existingPost != null) {
                _context.Entry(existingPost).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task LikePostAsync(int postId) {
            var post = await GetByIdAsync(postId);
            if (post != null) {
                post.LikeCount++;
                await UpdateAsync(post);
            }
        }
        public async Task UnLikePostAsync(int postId) {
            var post = await GetByIdAsync(postId);
            if (post != null) {
                post.LikeCount--;
                await UpdateAsync(post);
            }
        }
        public async Task<bool> AddCommentAsync(int postId) {
            var post = await GetByIdAsync(postId);
            if (post != null) {
                post.CommentCount++;
                await UpdateAsync(post);
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveCommentAsync(int postId) {
            var post = await GetByIdAsync(postId);
            if (post != null) {
                post.CommentCount--;
                await UpdateAsync(post);
                return true;
            }
            else
                return false;
        }
    }
}
