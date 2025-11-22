using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.Repository {
    public class PostRepository : IPostRepository {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<Post>> GetAllAsync() {
            return await _context.Posts.ToListAsync();
        }
        public async Task<Post?> GetByIdAsync(int id) {
            return await _context.Posts
                .Include(p => p.User)  
                .Include(p => p.Medias)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == id);
        }
        public async Task AddAsync(Post entity) {
            await _context.Posts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var post = await _context.Posts.FindAsync(id);
            if (post != null) {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Post entity) {
            var existingPost = await _context.Posts.FindAsync(entity.PostId);
            if (existingPost != null) {
                _context.Entry(existingPost).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
