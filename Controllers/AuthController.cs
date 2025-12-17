using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [AllowAnonymous]
    public class AuthController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.UserNameOrEmail,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded) {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]   
        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterEmailStepModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null) {
                ModelState.AddModelError("", "Email này đã được sử dụng.");
                return View(model);
            }

            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            HttpContext.Session.SetString("Regist_Email", model.Email);
            HttpContext.Session.SetString("Regist_OTP", otp);

            await _emailService.SendEmailAsync(model.Email, "Mã xác thực đăng ký",
                $"<h1>Mã OTP của bạn là: <b style='color:red'>{otp}</b></h1>");

            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Regist_Email")))
                return RedirectToAction("Register");

            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpStepModel model) {
            var serverOtp = HttpContext.Session.GetString("Regist_OTP");

            if (model.OtpCode == serverOtp) {
                HttpContext.Session.SetString("Regist_Verified", "true");
                return RedirectToAction("CompleteProfile");
            }
            else {
                ModelState.AddModelError("", "Mã OTP không đúng.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CompleteProfile() {
            if (HttpContext.Session.GetString("Regist_Verified") != "true")
                return RedirectToAction("Register");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(RegisterFinalStepModel model) {
            var email = HttpContext.Session.GetString("Regist_Email");
            var isVerified = HttpContext.Session.GetString("Regist_Verified");

            if (string.IsNullOrEmpty(email) || isVerified != "true")
                return RedirectToAction("Register");

            if (!ModelState.IsValid)
                return View(model);

            var user = new User {
                UserName = model.UserName,
                Email = email,
                FullName = model.FullName ?? "User", 
                Bio = model.Bio,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                HttpContext.Session.Clear();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string token) {
            if (userId == null || token == null)
                return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) {
                return View("VerifySuccess");
            }
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

       

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
