using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class PostController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        public PostController(UserManager<User> userManager, IPostService postService, ICommentService commentService) {
            _userManager = userManager;
            _postService = postService;
            _commentService = commentService;
        }
        private int GetCurrentUserId() {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdStr == null ? 0 : int.Parse(userIdStr);
        }
        [HttpGet]
        public IActionResult CreatePost() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostInputModel model) {
            if (!ModelState.IsValid) {
                return View();
            }
            int userId = int.Parse(_userManager.GetUserId(User));
            Console.WriteLine("User ID: " + userId);
            var newpost = await _postService.CreatePost(model, userId);
            if (newpost == null)
                return RedirectToAction("Index", "Home"); 
            return RedirectToAction("PostDetails", new { id = newpost.PostId });

        }
        [HttpGet]
        public async Task<IActionResult> PostDetails(int id) {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var post = await _postService.GetByIdAsync(id, userId);
            if (post == null)
                return NotFound();
            return View(post);
        }
        [HttpGet]
        public async Task<IActionResult> EditPost(int postId) {
            var post = await _postService.GetByIdAsync(postId, GetCurrentUserId());
            var model = new EditPostViewModel {
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                Hashtags = post.Hashtags,
                MediaFiles = post.Medias.Select(m => new PostMediaViewModel {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostInputModel model) {
            if (!ModelState.IsValid) {
                return RedirectToAction("PostDetails", new { id = model.PostId });
            }
            int userId = GetCurrentUserId();
            await _postService.EditPostAsync(model, userId);

            return RedirectToAction("PostDetails", new { id = model.PostId });
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId) {
            var userId = GetCurrentUserId();
            bool result = await _postService.DeletePostAsync(postId, userId);
            if (!result) {
                return BadRequest("Cannot delete post.");
            }
            return Ok();
        }

    }
}
