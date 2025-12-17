using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private readonly IPostService _postService;

        public HomeController(IPostService postService) {
            _postService = postService;
        }

        public async Task<IActionResult> Index() {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            int sessionSeed = Environment.TickCount;
            ViewBag.SessionSeed = sessionSeed;
            var posts = await _postService.GetNewsFeed(userId, 1, 10, sessionSeed);
            Console.WriteLine(posts.Count());
            Console.WriteLine("=======================================================================================================================================================================================");
            return View(posts.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int page, int seed) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var posts = await _postService.GetNewsFeed(userId, page, 10, seed);

            if (posts == null || !posts.Any()) {
                return NoContent(); 
            }

            return PartialView("_PostListPartial", posts.ToList());
        }

        private int GetCurrentUserId() {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdStr) ? 0 : int.Parse(userIdStr);
        }

        [HttpGet]
        public async Task<IActionResult> Memories() {
            var userId = GetCurrentUserId();
            var memories = await _postService.GetMemoriesAsync(userId);
            return View(memories);
        }

        [HttpGet("api/unread-counts")]
        public IActionResult GetUnreadCounts() {
            return Ok(new { notifications = 0, messages = 0 });
        }
    }
}
