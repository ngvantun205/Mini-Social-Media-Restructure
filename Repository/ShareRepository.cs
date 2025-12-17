using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class ShareRepository : IShareRepository {
        private readonly AppDbContext _context;
        public ShareRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddAsync(Share entity) {
            await _context.Shares.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var hashtag = await _context.Shares.FirstOrDefaultAsync(s => s.Id == id);
            if (hashtag != null) {
                _context.Shares.Remove(hashtag);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Share>> GetAllAsync() {
            return await _context.Shares.ToListAsync();
        }
        public async Task<Share?> GetByIdAsync(int id) {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Share>> GetFriendsShare(int userId) {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();
            followingIds.Add(userId);
            var shares = await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                    .ThenInclude(p => p.User)
                .Include(s => s.Post)
                    .ThenInclude(p => p.Medias)
                .Include(s => s.Post)
                    .ThenInclude(p => p.Likes)
                .Include(s => s.Post)
                    .ThenInclude(p => p.Comments)
                .Where(s => followingIds.Contains(s.UserId) &&
                            s.SharedAt >= DateTime.UtcNow.AddDays(-30))
                .ToListAsync();
            return shares;
        }
        public async Task UpdateAsync(Share entity) {
            _context.Shares.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
