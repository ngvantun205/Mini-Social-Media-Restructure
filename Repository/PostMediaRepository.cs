using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class PostMediaRepository : IPostMediaRepository {
        private readonly AppDbContext _context;
        public PostMediaRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddAsync(PostMedia entity) {
            await _context.PostMedias.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var postMedia = await _context.PostMedias.FindAsync(id);
            if (postMedia != null) {
                _context.PostMedias.Remove(postMedia);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<PostMedia>> GetAllAsync() {
            return await _context.PostMedias.ToListAsync();
        }
        public async Task<PostMedia?> GetByIdAsync(int id) {
            return await _context.PostMedias.FindAsync(id);
        }
        public async Task UpdateAsync(PostMedia entity) {
            _context.PostMedias.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<PostMedia?> GetByUrlAsync(string url) {
            return await _context.PostMedias.FirstOrDefaultAsync(pm => pm.Url == url.Trim());
        }
        public async Task RemoveRangeAsync(IEnumerable<PostMedia> postMedias) {
            _context.PostMedias.RemoveRange(postMedias);
            await _context.SaveChangesAsync();
        }
    }
}
