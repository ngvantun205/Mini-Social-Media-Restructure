
using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class StoryArchiveRepository : IStoryArchiveRepository {
        private readonly AppDbContext _context;
        public StoryArchiveRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(StoryArchive entity) {
            await _context.StoryArchives.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var strA = await _context.StoryArchives.FirstOrDefaultAsync(s => s.Id == id);
            if (strA != null) {
                _context.StoryArchives.Remove(strA);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StoryArchive>> GetUserStoryArchive(int userId) {
            return await _context.StoryArchives.Where(sa => sa.UserId == userId)
                .Include(sa => sa.User)
                .ToListAsync();
        }
    }
}
