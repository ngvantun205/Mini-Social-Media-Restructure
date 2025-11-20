using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.Repository {
    public class UserRepository : IUserRepository {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllAsync() {
            return await _context.Users.ToListAsync();
        }   
        public async Task<User?> GetByIdAsync(int id) {
            return await _context.Users.FindAsync(id);
        }
        public async Task AddAsync(User entity) {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var user = await _context.Users.FindAsync(id);
            if (user != null) {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }   
        public async Task UpdateAsync(User entity) {
            var existingUser = await _context.Users.FindAsync(entity.UserId);
            if (existingUser != null) {
                _context.Entry(existingUser).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
