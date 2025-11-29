using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    public class FollowController : Controller {
        private readonly IFollowService _followService;
        public FollowController(IFollowService followService) {
            _followService = followService;
        }
        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }
        [HttpPost]
        public async Task<IActionResult> Follow([FromBody] FollowInputModel inputModel) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _followService.Follow(inputModel, userId);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(new { success = false, error = result.ErrorMessage });

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow([FromBody] FollowInputModel inputModel) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _followService.Unfollow(inputModel, userId);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(new { success = false, error = result.ErrorMessage });

            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> FollowerList(int requesterId) {
            if (requesterId == 0)
                return BadRequest("Invalid User ID");
            var listfollower = await _followService.GetFollowers(requesterId);
            return Ok(listfollower);
        }
        [HttpGet]
        public async Task<IActionResult> FolloweeList(int requesterId) {
            if (requesterId == 0)
                return BadRequest("Invalid User ID");
            var listfollowee = await _followService.GetFollowee(requesterId);
                return Ok(listfollowee);
        }
    }
}
