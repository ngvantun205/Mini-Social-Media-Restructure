using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mini_Social_Media.Models;

namespace Mini_Social_Media.Controllers {
    public class HomeController : Controller {
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;
        public HomeController(IPostService postService, UserManager<User> userManager) {
            _postService = postService;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index() {
            var userid = _userManager.GetUserId(User);
            if (userid == null) return Unauthorized();
            int userId = int.Parse(userid); 
                var posts = await _postService.GetPostsPagedAsync(1, 10, userId);
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int page) {
            var userid = _userManager.GetUserId(User);
            if (userid == null)
                return Unauthorized();
            int userId = int.Parse(userid);
            var posts = await _postService.GetPostsPagedAsync(page, 10, userId);

            if (posts == null || !posts.Any()) {
                return NoContent();
            }

            return PartialView("_PostListPartial", posts);
        }

    }
}
