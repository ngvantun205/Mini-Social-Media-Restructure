using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class NotificationsRepository : INotificationsRepository {
        private readonly AppDbContext _context;
        public NotificationsRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<Notifications>> GetAllAsync() {
            return await _context.Notifications.ToListAsync();
        }
        public async Task<Notifications?> GetByIdAsync(int id) {
            return await _context.Notifications.FindAsync(id);
        }
        public async Task AddAsync(Notifications entity) {
            await _context.Notifications.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var noti = await _context.Notifications.FindAsync(id);
            if (noti != null) {
                _context.Notifications.Remove(noti);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Notifications entity) {
            var existingnoti = await _context.Notifications.FindAsync(entity.NotiId);
            if (existingnoti != null) {
                _context.Entry(existingnoti).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Notifications>> GetByReceiverIdAsync(int receiverId) {
            return await _context.Notifications
                .Where(n => n.ReceiverId == receiverId)
                .Include(n => n.Actor)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId) {
            var noti = await _context.Notifications.FindAsync(notificationId);
            if (noti != null && !noti.IsRead) {
                noti.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId) {
            var unreadNotis = await _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotis.Any()) {
                foreach (var noti in unreadNotis) {
                    noti.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Notifications?> GetNotification(int actorId, int receiverId, string type, int entityId) {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.ActorId == actorId && n.ReceiverId == receiverId && n.Type == type && n.EntityId == entityId);
        }
    }
}
