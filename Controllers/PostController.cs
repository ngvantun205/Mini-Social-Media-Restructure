using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    public class PostController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;
        public PostController(UserManager<User> userManager, IPostService postService) {
            _userManager = userManager;
            _postService = postService;
        }
        [HttpGet]
        public IActionResult CreatePost() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostInputModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            int userId = int.Parse(_userManager.GetUserId(User));
            Console.WriteLine("User ID: " + userId);
            var createPostDto = await _postService.CreatePost(model, userId);
            return RedirectToAction("PostDetails", new { id = createPostDto.PostId });

        }
        [HttpGet]
        public async Task<IActionResult> PostDetails(int id) {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            else {
                var postviewmodel = new PostViewModel {
                    UserId = post.UserId,
                    PostId = post.PostId,
                    Caption = post.Caption,
                    Location = post.Location,
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    CreatedAt = post.CreatedAt,
                    Medias = post.MediaUrls?.Select(url => new PostMediaViewModel {
                        Url = url,
                        MediaType = url.EndsWith(".mp4") ? "video" : "image"
                    }).ToList() ?? new List<PostMediaViewModel>(),
                    Hashtags = post.Hashtags,
                };

                return View(postviewmodel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditPost(int postId) {
            var post = await _postService.GetByIdAsync(postId);
            var model = new EditPostViewModel {
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                Hashtags = post.Hashtags,
                MediaFiles = post.MediaUrls?.ToList() ?? new List<string>()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostInputModel model) {
            if (!ModelState.IsValid)
                return View(model);

            int userId = int.Parse(_userManager.GetUserId(User));
            await _postService.EditPostAsync(model, userId);

            return RedirectToAction("PostDetails", new { id = model.PostId });
        }
        public async Task<IActionResult> DeletePost(int postId) {
            int userId = int.Parse(_userManager.GetUserId(User));
            bool result = await _postService.DeletePostAsync(postId, userId);
            if (!result) {
                return BadRequest("Không thể xóa bài viết.");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
