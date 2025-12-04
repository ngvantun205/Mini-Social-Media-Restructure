using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    public class SearchController : Controller {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        public SearchController(IUserService userService, IPostService postService) {
            _userService = userService;
            _postService = postService;
        }
        private int GetCurrentUserId() {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdStr == null ? 0 : int.Parse(userIdStr);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchinfo) {
            int userId = GetCurrentUserId();
            var users = await _userService.SearchUsers(searchinfo);
            List<PostViewModel> posts = await _postService.SearchPosts(searchinfo, userId);
            ViewData["Posts"] = posts;
            ViewData["Users"] = users;
            return View();
        }
    }
}
