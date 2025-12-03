using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;
        public HomeController(IPostService postService, UserManager<User> userManager) {
            _postService = postService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index() {
            var userid = _userManager.GetUserId(User);
            if (userid == null) return Unauthorized();
            int userId = int.Parse(userid);
            var posts = await _postService.GetPostsPagedAsync(1, 10, userId);
            return View(new List<PostViewModel>(posts.Select(post => new PostViewModel {
                Owner = new UserSummaryViewModel() {UserId = post.Owner.UserId, UserName = post.Owner.UserName, AvatarUrl = post.Owner.AvatarUrl, FullName = post.Owner.FullName },
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                LikeCount = post.LikeCount,
                IsLiked = post.IsLiked,
                CommentCount = post.CommentCount,
                CreatedAt = post.CreatedAt,                
                Medias = post.Medias.Select(m => new PostMediaViewModel {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList(),
                Hashtags = post.Hashtags,
            }).ToList()));
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

            return PartialView("_PostListPartial", new List<PostViewModel>(posts.Select(post => new PostViewModel {
                Owner = new UserSummaryViewModel() { UserId = post.Owner.UserId, UserName = post.Owner.UserName, AvatarUrl = post.Owner.AvatarUrl, FullName = post.Owner.FullName },
                PostId = post.PostId,
                Caption = post.Caption,
                Location = post.Location,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                CreatedAt = post.CreatedAt,
                IsLiked = post.IsLiked,
                Medias = post.Medias.Select(m => new PostMediaViewModel {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList(),
                Hashtags = post.Hashtags,
            }).ToList()));
        }

    }
}
