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
        public async Task<IEnumerable<Post>> SearchPost(string searchinfo) {
            return await _context.Posts.Where(p => p.Caption.ToLower().Contains(searchinfo.ToLower().Trim()))
                .Include(p => p.Medias)
                .Include(p => p.PostHashtags).ThenInclude(ph => ph.Hashtag)
                .Include(p => p.User)
                .ToListAsync();
        }
        public async Task<List<Post>> GetNewsFeedPosts(int userId) {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();
            followingIds.Add(userId);
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Medias)
                .Include(p => p.Likes)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .Where(p => followingIds.Contains(p.UserId) &&
                            p.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .ToListAsync();

            return posts;
        }
        public async Task<List<Post>> GetSuggestedPosts(int userId, int limit) {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();
            followingIds.Add(userId);
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Medias)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => !followingIds.Contains(p.UserId) && 
                            p.CreatedAt >= DateTime.UtcNow.AddDays(-15))
                .OrderByDescending(p => p.Likes.Count) 
                .ThenByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetMemoriesAsync(int userId) {
            var today = DateTime.UtcNow;

            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Medias)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.UserId == userId
                            && p.CreatedAt.Month == today.Month
                            && p.CreatedAt.Day == today.Day
                            && p.CreatedAt.Year < today.Year) 
                .OrderByDescending(p => p.CreatedAt) 
                .ToListAsync();
        }
        public async Task<string> GetOwnerUsername(int postId) {
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            return post?.User?.UserName;
        }
    }
}
