using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class AdController : Controller {
        private readonly IAdService _adService;
        public AdController(IAdService adService) {
            _adService = adService;
        }
        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }
        [HttpGet]
        public IActionResult RequestAd() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestAd(AdInputModel adInputModel) {
            if (!ModelState.IsValid)
                return View(adInputModel);

            var userId = GetCurrentUserId();
            await _adService.RequestAd(adInputModel, userId);
            return RedirectToAction(nameof(UserAd));
        }
        [HttpGet]
        public async Task<IActionResult> UserAd() {
            var userId = GetCurrentUserId();
            var myAd = await _adService.GetUserAd(userId);
            return View(myAd);
        }
        [HttpGet]
        public async Task<IActionResult> EditAd(int adId) {
            var ad = await _adService.GetById(adId);
            return View(ad);
        }
        [HttpPost]
        public async Task<IActionResult> EditAd(EditAdInputModel editAdInputModel) {
            if (!ModelState.IsValid)
                return View(editAdInputModel);
            var userId = GetCurrentUserId();
            await _adService.UpdateAd(editAdInputModel, userId);
            return RedirectToAction(nameof(UserAd));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveAd(int adId) {
            var userId = GetCurrentUserId();
            await _adService.ApprovedAd(adId, userId);
            return Ok();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeclineAd(int adId) {
            var userId = GetCurrentUserId();
            await _adService.DeclineAd(adId, userId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int adId) {
            var userId = GetCurrentUserId();
            await _adService.CancelRequest(adId, userId);
            return RedirectToAction(nameof(UserAd));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageAds(string status = "Pending") {
            var userId = GetCurrentUserId();
            var ads = await _adService.GetAdsByStatusForAdmin(userId, status);

            return Ok(ads);
        }
    }
}
