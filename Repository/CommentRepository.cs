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
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .Include(c => c.Post)
                .Include(c => c.ParentComment)
                .FirstOrDefaultAsync(c => c.CommentId == id);
        }
        public async Task AddAsync(Comment entity) {
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment != null) {
                if (comment.Replies != null && comment.Replies.Any()) {
                    _context.Comments.RemoveRange(comment.Replies);
                }
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
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId) {
            return await _context.Comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Include(c => c.User)
                .ToListAsync();
        }
        public async Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(int commentId) {
            return await _context.Comments
                .Where(c => c.ParentCommentId == commentId)
                .Include(c => c.User)
                .ToListAsync();
        }
        public async Task<bool> AddReplyAsync(int commentId) {
            var comment = await GetByIdAsync(commentId);
            if (comment == null) return false;
            comment.ReplyCount++;
            await UpdateAsync(comment);
            return true;
        }
        public async Task<bool> RemoveReplyAsync(int commentId) {
            var comment = await GetByIdAsync(commentId);
            if (comment == null) return false;
            comment.ReplyCount--;
            await UpdateAsync(comment);
            return true;
        }
    }
}
