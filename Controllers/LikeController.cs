using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mini_Social_Media.Controllers {
    public class LikeController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly ILikeService _likeService;
        public LikeController(UserManager<User> userManager, ILikeService likeService) {
            _userManager = userManager;
            _likeService = likeService;
        }
        [HttpPost]
        public async Task<IActionResult> LikeAsync([FromBody] LikeInputModel likeinput) 
{
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            var userId = int.Parse(userIdStr);

            var likedto = await _likeService.ToggleLikeAsync(likeinput, userId);

            if (!string.IsNullOrEmpty(likedto.ErrorMessage)) {
                return BadRequest(likedto.ErrorMessage);
            }

            return Ok(new { success = true });
        }
    }
}
