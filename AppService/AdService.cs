using Microsoft.AspNetCore.Identity;
using Mini_Social_Media.Models.InputModel;
using Mini_Social_Media.Repository;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mini_Social_Media.AppService {
    public class AdService : IAdService { 
        private readonly IAdRepository _adRepository;
        private readonly IUploadService _uploadService;
        private readonly UserManager<User> _userManager;
        public AdService(IAdRepository adRepository, IUploadService uploadService, UserManager<User> userManager) {
            _adRepository = adRepository;
            _uploadService = uploadService;
            _userManager = userManager;
        }
        private async Task<bool> IsAdmin(int userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains("Admin");
        }
        public async Task<bool> ApprovedAd(int adId, int userId) {
            if (await IsAdmin(userId)) {
                var ad = await _adRepository.ApproveAd(adId);
                return true;
            }
            return false;
        }
        public async Task<bool> DeclineAd(int adId, int userId) {
            if (await IsAdmin(userId)) {
                var ad = await _adRepository.DeclineAd(adId);
                return true;
            }
            return false;
        }
        public async Task<bool> DisableAd(int adId, int userId) {
            if (await IsAdmin(userId)) {
                var ad = await _adRepository.DisableAd(adId);
                return true;
            }
            return false;
        }

        public async Task RequestAd(AdInputModel inputModel, int userId) {
            var url = await _uploadService.UploadAsync(inputModel.AdMedia);
            var ad = new Advertisement() {
                UserId = userId,
                Type = inputModel.Type,
                Status = AdStatus.Pending,
                Title = inputModel.Title,
                Content = inputModel.Content,
                TargetUrl = inputModel.TargetUrl,
                CtaText = inputModel.CtaText,
                StartDate = inputModel.StartDate,
                EndDate = inputModel.EndDate,
                Budget = inputModel.Budget,
                ImageUrl = url,
                CreatedAt = DateTime.UtcNow
            };
            await _adRepository.AddAsync(ad);
        }

        public async Task<bool> UpdateAd(EditAdInputModel inputModel, int userId) {
            var ad = await _adRepository.GetByIdAsync(inputModel.Id);
            if (ad == null || userId != ad.UserId)
                return false;

            if (inputModel.AdMedia != null && inputModel.AdMedia.Length > 0) {
                var url = await _uploadService.UploadAsync(inputModel.AdMedia);
                ad.ImageUrl = url;
            }

            ad.Type = inputModel.Type;
            ad.Title = inputModel.Title;
            ad.Content = inputModel.Content;
            ad.TargetUrl = inputModel.TargetUrl;
            ad.Budget = inputModel.Budget;
            ad.StartDate = inputModel.StartDate;
            ad.EndDate = inputModel.EndDate;
            ad.CtaText = inputModel.CtaText;
            ad.UpdateAt = DateTime.UtcNow;

            await _adRepository.UpdateAsync(ad);
            return true;
        }
        public async Task<IEnumerable<AdViewModel>> GetUserAd(int userId) {
            var ads = await _adRepository.GetUserAd(userId);
            if (ads == null)
                return new List<AdViewModel>();
            return ads.Select(ad => new AdViewModel {
                Id = ad.Id,
                Brand = new UserSummaryViewModel { UserId = ad.UserId, UserName = ad.User.UserName, AvatarUrl = ad.User.AvatarUrl, FullName = ad.User.FullName },
                Type = ad.Type,
                Status = ad.Status,
                Title = ad.Title,
                Content = ad.Content,
                ImageUrl = ad.ImageUrl,
                TargetUrl = ad.TargetUrl,
                CtaText = ad.CtaText,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Budget = ad.Budget,
                IsPaid = ad.IsPaid,
                TotalClicks = ad.TotalClicks,
                TotalImpressions = ad.TotalImpressions,
                UpdateAt = ad.UpdateAt,
            }).ToList();
        }

        public async Task<bool> CancelRequest(int adId, int userId) {
            var ad = await _adRepository.GetByIdAsync(adId);
            if (userId == ad.UserId) {
                await _adRepository.CancelRequestAd(adId);
                return true;
            }
            return false;
        }

        public async Task<AdViewModel> GetById(int adId) {
            var ad = await _adRepository.GetByIdAsync(adId);
            return new AdViewModel {
                Id = ad.Id,
                Brand = new UserSummaryViewModel { UserId = ad.UserId, UserName = ad.User.UserName, AvatarUrl = ad.User.AvatarUrl, FullName = ad.User.FullName },
                Type = ad.Type,
                Status = ad.Status,
                Title = ad.Title,
                Content = ad.Content,
                ImageUrl = ad.ImageUrl,
                TargetUrl = ad.TargetUrl,
                CtaText = ad.CtaText,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Budget = ad.Budget,
                IsPaid = ad.IsPaid,
                TotalClicks = ad.TotalClicks,
                TotalImpressions = ad.TotalImpressions,
                UpdateAt = ad.UpdateAt,
            };
        }
        public async Task<IEnumerable<AdViewModel>> GetAdsByStatusForAdmin(int userId, string statusStr) {
            if (!await IsAdmin(userId))
                return new List<AdViewModel>();

            if (!Enum.TryParse(statusStr, true, out AdStatus status)) {
                return new List<AdViewModel>();
            }
            var ads = await _adRepository.GetAdsByStatusAsync(status);

            return ads.Select(ad => new AdViewModel {
                Id = ad.Id,
                Brand = new UserSummaryViewModel { UserName = ad.User.UserName, AvatarUrl = ad.User.AvatarUrl },
                Title = ad.Title,
                Content = ad.Content,
                ImageUrl = ad.ImageUrl,
                Budget = ad.Budget,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Status = ad.Status
            }).ToList();
        }

        public async Task<AdViewModel> GetRandomBanner() {
            var ad = await _adRepository.GetRandomBanner();
            if (ad == null) return new AdViewModel { };
            return new AdViewModel {
                Id = ad.Id,
                Brand = new UserSummaryViewModel { UserName = ad.User.UserName, AvatarUrl = ad.User.AvatarUrl },
                Title = ad.Title,
                Content = ad.Content,
                ImageUrl = ad.ImageUrl,
                Budget = ad.Budget,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Status = ad.Status
            };
        }
    }
}
