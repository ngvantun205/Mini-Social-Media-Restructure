using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class ProfileController : Controller {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public ProfileController(IUserService userService, UserManager<User> userManager) {
            _userService = userService;
            _userManager = userManager;
        }
        private int GetCurrentUserId() {
            string userIdString = _userManager.GetUserId(User);
            if (userIdString == null) {
                return 0;
            }
            return int.Parse(userIdString);
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
            return View(new MyProfileViewModel() {
                AvatarUrl = profile.AvatarUrl,
                Email = profile.Email,
                UserName = profile.UserName,
                FullName = profile.FullName,
                Bio = profile.Bio,
                WebsiteUrl = profile.WebsiteUrl,
                IsPrivate = profile.IsPrivate,
                Gender = profile.Gender,
                FollowerCount = profile.FollowersCount,
                FollowingCount = profile.FollowingCount,
                IsOwner = profile.IsOwner,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                Posts = profile.Posts.Select(p => new PostSummaryViewModel {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    MediaUrl = p.MediaUrl
                }).ToList()
            });
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
            return View(new UserProfileViewModel() {
                UserId = profile.UserId,
                UserName = profile.UserName,
                FullName = profile.FullName,
                Bio = profile.Bio,
                AvatarUrl = profile.AvatarUrl,
                WebsiteUrl = profile.WebsiteUrl,
                IsPrivate = profile.IsPrivate,
                CreatedAt = profile.CreatedAt,
                FollowerCount = profile.FollowersCount,
                FollowingCount = profile.FollowingCount,
                IsFollowing = profile.IsFollowing,
                Gender = profile.Gender,
                Posts = profile.Posts.Select(p => new PostSummaryViewModel {
                    PostId = p.PostId,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    MediaUrl = p.MediaUrl
                }).ToList()
            });
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
    }
}
