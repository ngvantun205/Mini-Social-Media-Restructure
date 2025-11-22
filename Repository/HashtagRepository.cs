using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class HashtagRepository : IHashtagRepository {
        private readonly AppDbContext _context;
        public HashtagRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddAsync(Hashtag entity) {
            await _context.Hashtags.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var hashtag = await _context.Hashtags.FindAsync(id);
            if (hashtag != null) {
                _context.Hashtags.Remove(hashtag);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Hashtag>> GetAllAsync() {
            return await _context.Hashtags.ToListAsync();
        }
        public async Task<Hashtag?> GetByIdAsync(int id) {
            return await _context.Hashtags.FindAsync(id);
        }
        public async Task<bool> IsExist(string hashtagName) {
            return await _context.Hashtags.AnyAsync(h => h.HashtagName == hashtagName.ToLower());
        }
        public async Task UpdateAsync(Hashtag entity) {
            _context.Hashtags.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<Hashtag?> GetByNameAsync(string hashtagName) {
            return await _context.Hashtags
                .FirstOrDefaultAsync(h => h.HashtagName == hashtagName.ToLower());
        }
    }
}
