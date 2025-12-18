using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class ProfileController : Controller {
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        public ProfileController(IUserService userService, ICommentService commentService, ILikeService likeService) {
            _userService = userService;
            _commentService = commentService;
            _likeService = likeService;
        }
        private int GetCurrentUserId() {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdString == null ? 0 : int.Parse(userIdString);
        }
        [HttpGet]
        public async Task<IActionResult> MyProfile() {
            int userId = GetCurrentUserId();
            if (userId == 0) {
                return Unauthorized();
            }
            var profile = await _userService.GetMyProfileAsync(userId);
            if (profile == null) {
                return NotFound();
            }
            return View(profile);
        }
        [HttpGet]
        public async Task<IActionResult> UserProfile(int id) {
            int requesterId = GetCurrentUserId();
            if (requesterId == 0) {
                return Unauthorized();
            }
            var profile = await _userService.GetUserProfileAsync(id, requesterId);
            if (profile == null) {
                return NotFound();
            }
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile) {
            var userId = GetCurrentUserId(); 
            if (userId == 0)
                return Unauthorized();

            if (avatarFile == null || avatarFile.Length == 0) {
                return BadRequest("File is empty.");
            }

            var profileDto = await _userService.UpdateUserAvatar(avatarFile, userId);

            if (profileDto == null) {
                return BadRequest("Failed to update avatar.");
            }

            return Ok(new { newUrl = profileDto.AvatarUrl });
        }

        [HttpGet("Profile/GetUserInfo/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId) {
            var user = await _userService.GetMyProfileAsync(userId); 

            if (user == null)
                return NotFound();

            return Ok(new {
                id = userId,
                userName = user.UserName,
                avatarUrl = user.AvatarUrl ?? "/images/avatar.png"
            });
        }
        [HttpGet] 
        public async Task<IActionResult> RecentActivity() {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var likeHistory = (await _likeService.GetUserHistoryLike(userId)).ToList();
            Console.WriteLine("======================================================================================================================================");
            Console.WriteLine(likeHistory.Count());
            var commentHistory = (await _commentService.GetUserHistoryComment(userId)).ToList();
            Console.WriteLine("======================================================================================================================================");
            Console.WriteLine(commentHistory.Count());
            ViewBag.LikeHistory = likeHistory;
            ViewBag.CommentHistory = commentHistory;
            return View();
        }
    }
}
