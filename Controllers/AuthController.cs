using Microsoft.AspNetCore.Mvc;

namespace Mini_Social_Media.Controllers {
    public class AuthController : Controller {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login() {
            var userId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"Log Information User ID: {userId}");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model) {
            if (!ModelState.IsValid) {
                return View(model); 
            }
            var result = await _authService.LoginAsync(model);
            if (!string.IsNullOrEmpty(result.ErrorMessage)) {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }
            string token = result.Token!;
            var cookieOptions = new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            // var token = Request.Cookies["jwt"]; - how to read cookie
            Response.Cookies.Append("jwt", token, cookieOptions);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel model) {
            if (!ModelState.IsValid) {
                return View(model); 
            }
            var result = await _authService.RegisterAsync(model);
            if (!string.IsNullOrEmpty(result.ErrorMessage)) {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }
            return RedirectToAction("Login", "Auth");
        }
        [HttpGet]
        public async Task<IActionResult> Logout() {
            Response.Cookies.Delete("jwt");
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}
