using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class LikeRepository : ILikeRepository {
        private readonly AppDbContext _context;
        public LikeRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<Like>> GetAllAsync() {
            return await _context.Likes.ToListAsync();
        }
        public async Task<Like?> GetByIdAsync(int id) {
            return await _context.Likes.FindAsync(id);
        }
        public async Task AddAsync(Like entity) {
            await _context.Likes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var like = await _context.Likes.FindAsync(id);
            if (like != null) {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Like entity) {
            var existingLike = await _context.Likes.FindAsync(entity.LikeId);
            if (existingLike != null) {
                _context.Entry(existingLike).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteByPostIdAndUserIdAsync(int postId, int userId) {
            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like != null) {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> IsLikedByCurrentUser(int postId, int userId) {
            return await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task<IEnumerable<Like>> GetUserHistoryLike(int userId) {
            return await _context.Likes.Where(l => l.UserId != userId).Include(l => l.User).Include(l => l.Post).ToListAsync();
        }
    }
}
