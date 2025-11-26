using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Social_Media.Models;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class CommentController : Controller {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService) {
            _commentService = commentService;
        }

        private int GetCurrentUserId() {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return 0;
            return int.Parse(userIdStr);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentInputModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _commentService.AddCommentAsync(model, userId);

            if (result == null) {
                return BadRequest("Failed to add comment.");
            }
            return Ok(result); 
        }

        [HttpPost] 
        public async Task<IActionResult> DeleteComment([FromBody] int commentId) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _commentService.DeleteCommentAsync(commentId, userId);

            if (!result) {
                return BadRequest("Failed to delete comment or unauthorized.");
            }
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCommentById(int commentId) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _commentService.DeleteCommentAsync(commentId, userId);
            return result ? Ok() : BadRequest();
        }

        [HttpPut] 
        public async Task<IActionResult> EditComment([FromBody] EditCommentInputModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _commentService.EditCommentAsync(model, userId);

            if (result == null) {
                return BadRequest("Failed to edit comment or unauthorized.");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ReplyComment([FromBody] ReplyCommentInputModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _commentService.AddReplyAsync(model, userId);

            if (result == null) {
                return BadRequest("Failed to add reply.");
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("Comment/GetReplies/{commentId}")]
        public async Task<IActionResult> GetReplies(int commentId) {
            var replies = await _commentService.GetRepliesByCommentIdAsync(commentId);

            return Ok(replies.Select(r => new CommentViewModel {
                CommentId = r.CommentId,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                ReplyCount = r.ReplyCount,
                Owner = new UserSummaryViewModel {
                    UserId = r.Owner.UserId,
                    UserName = r.Owner.UserName,
                    FullName = r.Owner.FullName,
                    AvatarUrl = r.Owner.AvatarUrl
                }
            }).ToList());
        }

    }
}