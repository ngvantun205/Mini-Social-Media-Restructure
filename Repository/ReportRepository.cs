using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Repository {
    public class ReportRepository : IReportRepository {
        private readonly AppDbContext _context;
        public ReportRepository(AppDbContext context) {
            _context = context;
        }
        public async Task AddAsync(Report entity) {
            await _context.Reports.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id) {
            var report = await _context.Reports.FindAsync(id);
            if (report != null) {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Report>> GetAllAsync() {
            return await _context.Reports.ToListAsync();
        }
        public async Task<Report?> GetByIdAsync(int id) {
            return await _context.Reports
                .Include(r => r.Reporter)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }
        public async Task UpdateAsync(Report entity) {
            _context.Reports.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Report>> FilterReportByType(string type) {
            return await _context.Reports.Where(r => r.Type == type).ToListAsync();
        }
        public async Task<IEnumerable<Report>> FilterReportsByStatus(ReportStatus status) {
            bool isExecuted = (status == ReportStatus.Executed);

            return await _context.Reports
                .Where(r => r.IsExecuted == isExecuted)
                .Include(r => r.Reporter)
                .ToListAsync();
        }
    }
}
