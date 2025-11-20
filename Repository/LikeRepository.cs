using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.IRepository;
using Mini_Social_Media.Models.DomainModel;

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
    }
}
