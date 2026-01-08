using Microsoft.EntityFrameworkCore;
namespace Mini_Social_Media.Repository {
    public class LiveStreamRepository : ILiveStreamRepository {
        private readonly AppDbContext _context;
        public LiveStreamRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(LiveStream entity) {
            await _context.LiveStreams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LiveStream entity) {
            _context.LiveStreams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var ls = await _context.LiveStreams.FirstOrDefaultAsync(x => x.Id == id);
            if (ls != null) {
                _context.LiveStreams.Remove(ls);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LiveStream?> GetByIdAsync(int id) {
            return await _context.LiveStreams
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<LiveStream>> GetActiveStreamsAsync() {
            return await _context.LiveStreams
                .Include(x => x.User)
                .Where(x => x.Status == LiveStreamStatus.OnAir)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }
        public async Task<LiveStream?> GetCurrentStreamByUserIdAsync(int userId) {
            return await _context.LiveStreams
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == LiveStreamStatus.OnAir);
        }

        public Task<IEnumerable<LiveStream>> GetAllAsync() {
            throw new NotImplementedException();
        }
    }
}