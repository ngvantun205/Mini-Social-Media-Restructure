using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.Data;
using Mini_Social_Media.Models;

namespace Mini_Social_Media.Repository {
    public class LiveChatMessageRepository : ILiveChatMessageRepository {
        private readonly AppDbContext _context;
        public LiveChatMessageRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddAsync(LiveChatMessage entity) {
            await _context.LiveChatMessages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<LiveChatMessage>> GetByLiveStreamId(int liveStreamId) {
            return await _context.LiveChatMessages
                .Include(lc => lc.User) 
                .Where(lc => lc.LiveStreamId == liveStreamId)
                .OrderBy(lc => lc.SentAt)
                .ToListAsync();
        }
    }
}