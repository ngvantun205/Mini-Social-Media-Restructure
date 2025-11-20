using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.IRepository;
using Mini_Social_Media.Models.DomainModel;

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
            return await _context.Follows.FindAsync(id);
        }
        public async Task AddAsync(Follow entity) {
            await _context.Follows.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var follow = await _context.Follows.FindAsync(id);
            if (follow != null) {
                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Follow entity) {
            var existingFollow = await _context.Follows.FindAsync(entity.FollowId);
            if (existingFollow != null) {
                _context.Entry(existingFollow).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
