using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.IRepository;

namespace Mini_Social_Media.Repository {
    public class CommentRepository : ICommentRepository {
        private readonly AppDbContext _context;
        public CommentRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> GetAllAsync() {
            return await _context.Comments.ToListAsync();
        }
        public async Task<Comment?> GetByIdAsync(int id) {
            return await _context.Comments.FindAsync(id);
        }
        public async Task AddAsync(Comment entity) {
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null) {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Comment entity) {
            var existingComment = await _context.Comments.FindAsync(entity.CommentId);
            if (existingComment != null) {
                _context.Entry(existingComment).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
