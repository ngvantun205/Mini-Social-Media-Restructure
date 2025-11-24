using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}