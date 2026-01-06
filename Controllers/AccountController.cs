using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mini_Social_Media.IAppService;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    public class AccountController : Controller {
        private readonly IUserService _userService;
        public AccountController(IUserService userService) {
            _userService = userService;
        }
        private int GetCurrentUserId() {
           var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }
        [HttpGet]
        public async Task<IActionResult> Edit() {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var currentuser = await _userService.GetEditProfile(userId);
            return View(new EditProfileViewModel() {
                FullName = currentuser.FullName,
                Bio = currentuser.Bio,
                AvatarUrl = currentuser.AvatarUrl,
                WebsiteUrl = currentuser.WebsiteUrl,
                Gender = currentuser.Gender,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileInputModel editProfileInput) {
            if (!ModelState.IsValid)
                return View();
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var updated = await _userService.Edit(editProfileInput, userId);
            return View(new EditProfileViewModel() {
                FullName = updated.FullName,
                Bio = updated.Bio,
                AvatarUrl = updated.AvatarUrl,
                WebsiteUrl = updated.WebsiteUrl,
                Gender = updated.Gender,
            });
        }
        [HttpGet]
        public IActionResult ChangePassword() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel model) {
            if (!ModelState.IsValid)
                return View(model);

            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _userService.ChangePassword(model, userId);

            if (result.Succeeded)
                ViewBag.Success = true;
            else
                ViewBag.Errors = result.Errors.Select(e => e.Description).ToList();

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AccountPrivacy() {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var profile = await _userService.GetMyProfileAsync(userId);
            var IsPrivate = profile?.IsPrivate ?? false;
            return View(IsPrivate);
        }

        [HttpPost]
        public async Task<IActionResult> AccountPrivacy([FromBody] PrivacyInputModel model) {
            if (model == null)
                return BadRequest("model null");
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            await _userService.ChangeAccountPrivacy(model.IsPrivate, userId);
            return Ok(new { success = true });
        }

    }
}
