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
            if (string.IsNullOrEmpty(userIdStr))
                return 0;
            return int.Parse(userIdStr);
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
            var createPostDto = await _postService.CreatePost(model, userId);
            return RedirectToAction("PostDetails", new { id = createPostDto.PostId });

        }
        [HttpGet]
        public async Task<IActionResult> PostDetails(int id) {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var post = await _postService.GetByIdAsync(id, userId);
            if (post == null)
                return NotFound();
            var postviewmodel = new PostViewModel {
                Owner = new UserSummaryViewModel() { UserId = post.Owner.UserId, UserName = post.Owner.UserName, AvatarUrl = post.Owner.AvatarUrl, FullName = post.Owner.FullName },
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                CreatedAt = post.CreatedAt,
                IsLiked = post.IsLiked,
                Medias = post.MediaUrls.Select(m => new PostMediaViewModel {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList(),
                Hashtags = post.Hashtags,

                Comments = (await _commentService.GetCommentsByPostIdAsync(post.PostId)).Select(c => new CommentViewModel {
                    CommentId = c.CommentId,
                    Owner = new UserSummaryViewModel() {UserId = c.Owner.UserId, AvatarUrl = c.Owner.AvatarUrl, FullName = c.Owner.FullName, UserName = c.Owner.UserName },
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    CreatedAt = c.CreatedAt,
                    ReplyCount = c.ReplyCount
                }).OrderBy(c => c.CreatedAt).ToList() ?? new List<CommentViewModel>()
            };

            return View(postviewmodel);
        }
        [HttpGet]
        public async Task<IActionResult> EditPost(int postId) {
            var post = await _postService.GetByIdAsync(postId, GetCurrentUserId());
            var model = new EditPostViewModel {
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                Hashtags = post.Hashtags,
                MediaFiles = post.MediaUrls.Select(m => new PostMediaViewModel {
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
            int userId = int.Parse(_userManager.GetUserId(User));
            await _postService.EditPostAsync(model, userId);

            return RedirectToAction("PostDetails", new { id = model.PostId });
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId) {
            var userIdStr = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            bool result = await _postService.DeletePostAsync(postId, userId);

            if (!result) {
                return BadRequest("Cannot delete post.");
            }
            return Ok();
        }

    }
}
