using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    public class PostController : Controller {
        private readonly IPostService _postService;
        public PostController(IPostService postService) {
            _postService = postService;
        }
        [HttpGet]
        public IActionResult CreatePost() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostInputModel model) {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            if (!ModelState.IsValid) {
                return View(model);
            }
            var createPostDto = await _postService.CreatePost(model,userId);
            return RedirectToAction("PostDetails", new { id = createPostDto.PostId });
            
        }
        [HttpGet]
        public async Task<IActionResult> PostDetails(int id) {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            return View(post);
        }
    }
}
