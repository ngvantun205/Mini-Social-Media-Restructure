
using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class StoryRepository : IStoryRepository {
        private readonly AppDbContext _context;
        public StoryRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(Story entity) {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var str = await _context.Stories.FirstOrDefaultAsync(s => s.Id == id);
            if (str != null) {
                _context.Stories.Remove(str);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Story>> GetAllAsync() {
            return await _context.Stories.ToListAsync();
        }

        public async Task<Story?> GetByIdAsync(int id) {
            return await _context.Stories
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id); 
        }

        public async Task<IEnumerable<Story>> GetCurrentStories(int userId) {
            return await _context.Stories.Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
                .Include(s => s.User)
                .ToListAsync();
        }

        public Task UpdateAsync(Story entity) {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<Story>> GetFriendsStories(int currentUserId) {
            var followingIds = _context.Follows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FolloweeId);
            return await _context.Stories
                .Where(s => (followingIds.Contains(s.UserId) || s.UserId == currentUserId)
                            && s.ExpiresAt > DateTime.UtcNow)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
