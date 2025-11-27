using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mini_Social_Media.IAppService;

namespace Mini_Social_Media.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        public AccountController(UserManager<User> userManager, IUserService userService) {
            _userManager = userManager;
            _userService = userService;
        }
        private int GetCurrentUserId() {
            string userIdstr = _userManager.GetUserId(User);
            if (userIdstr == null)
                return 0;
            else return int.Parse(userIdstr);
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
            int userId = GetCurrentUserId();    
            if(userId == 0) return Unauthorized();
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

    }
}
