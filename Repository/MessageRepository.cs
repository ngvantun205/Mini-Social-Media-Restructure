using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class MessageRepository : IMessageRepository {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddMessageAsync(Messages message) {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Conversations?> GetConversationAsync(int userId1, int userId2) {
            return await _context.Conversations
                .Include(c => c.User2)
                .Include(c => c.User1)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == userId1 && c.User2Id == userId2) ||
                    (c.User1Id == userId2 && c.User2Id == userId1));
        }

        public async Task CreateConversationAsync(Conversations conversation) {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateConversationAsync(Conversations conversation) {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Messages>> GetMessagesAsync(int conversationId) {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversations>> GetUserConversationsAsync(int userId) {
            return await _context.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Include(c => c.User1) 
                .Include(c => c.User2)
                .Include(c => c.LatestMessage)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }
        public async Task MarkAsRead(int userId, int partnerId) {
            var messages = await _context.Messages.Where(m => m.SenderId == userId && m.ReceiverId ==  partnerId && !m.IsRead).ToListAsync();
            foreach (var message in messages) message.IsRead = true;
        }
    }
}
