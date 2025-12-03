using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class FollowRepository : IFollowRepository {
        private readonly AppDbContext _context;

        public FollowRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Follow>> GetAllAsync() {
            return await _context.Follows.ToListAsync();
        }

        public async Task<Follow?> GetByIdAsync(int id) {
            return await _context.Follows
                .Include(f => f.Follower)
                .Include(f => f.Followee)
                .FirstOrDefaultAsync(f => f.FollowId == id);
        }

        public async Task AddAsync(Follow entity) {
            await _context.Follows.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowId == id);
            if (follow != null) {
                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Follow entity) {
            var existing = await _context.Follows.FirstOrDefaultAsync(f => f.FollowId == entity.FollowId);
            if (existing != null) {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Follow?> GetFollowByUserAsync(int followeeId, int followerId) {
            return await _context.Follows
                .Include(f => f.Follower)
                .Include(f => f.Followee)
                .FirstOrDefaultAsync(f => f.FolloweeId == followeeId && f.FollowerId == followerId);
        }

        public async Task<IEnumerable<Follow>> GetFollowerAsync(int userId) {
            return await _context.Follows
                .Include(f => f.Follower)
                .Where(f => f.FolloweeId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Follow>> GetFolloweeAsync(int userId) {
            return await _context.Follows
                .Include(f => f.Followee)
                .Where(f => f.FollowerId == userId)
                .ToListAsync();
        }
    }
}
