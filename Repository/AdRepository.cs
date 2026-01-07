
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Mini_Social_Media.Repository {
    public class AdRepository : IAdRepository {
        private readonly AppDbContext _context;
        public AdRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(Advertisement entity) {
            await _context.Advertisements.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ApproveAd(int adId) {
           var ad = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return false;
            ad.Status = AdStatus.Running;
            ad.IsPaid = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineAd(int adId) {
            var ad = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return false;
            ad.Status = AdStatus.Rejected;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelRequestAd(int adId) {
            var ad = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return false;
            ad.Status = AdStatus.Canceled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id) {
            var ad = await _context.Advertisements.FirstOrDefaultAsync(a => a.Id == id);
            if (ad != null) {
                _context.Advertisements.Remove(ad);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> DisableAd(int adId) {
            var ad = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return false;
            ad.Status = AdStatus.Ended;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Advertisement>> GetAllAsync() {
            return await _context.Advertisements.ToListAsync();
        }

        public async Task<Advertisement?> GetByIdAsync(int id) {
            return await _context.Advertisements
                .Include(a => a.User)
                .FirstOrDefaultAsync(ad => ad.Id == id);
        }

        public async Task<IEnumerable<Advertisement>> GetUserAd(int userId) {
            return await _context.Advertisements.Include(a => a.User).Where(ad => ad.UserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(Advertisement entity) {
            var ad = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.Id ==  entity.Id);
            if (ad != null) {
                _context.Advertisements.Entry(ad).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Advertisement>> GetAdsByStatusAsync(AdStatus status) {
            return await _context.Advertisements
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Advertisement?> GetRandomBanner() {
            var activeAds = await _context.Advertisements
                .Include(a => a.User) 
                .Where(a => a.Status == AdStatus.Running
                         && a.IsPaid == true
                         && a.Type == AdType.Banner
                         && a.StartDate <= DateTime.UtcNow
                         && a.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            if (activeAds == null || activeAds.Count == 0) {
                return null;
            }

            var random = new Random();
            int index = random.Next(activeAds.Count);

            return activeAds[index];
        }

    }
}
