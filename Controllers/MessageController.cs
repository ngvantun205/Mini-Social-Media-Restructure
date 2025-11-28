using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class MessageController : Controller {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService) {
            _messageService = messageService;
        }

        private int GetCurrentUserId() {
            var str = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations() {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var list = await _messageService.GetUserConversationsAsync(userId);
            return Ok(list ?? new List<ConversationViewModel>());
        }

        [HttpGet("/Message/History/{partnerId}")]
        public async Task<IActionResult> GetMessageHistory(int partnerId) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var messages = await _messageService.GetMessageHistoryAsync(userId, partnerId);
            return Ok(messages ?? new List<MessageViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageInputModel input) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(input.Content))
                return BadRequest("Empty message");

            var result = await _messageService.SendMessageAsync(
                userId,
                input.ReceiverId,
                input.Content
            );

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> StartChat(int partnerId) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var conv = await _messageService.GetOrCreateConversationAsync(userId, partnerId);
            if (conv != null)
                return RedirectToAction("Index", new { conversationId = conv.ConversationId });

            return RedirectToAction("Index", new { partnerId });
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int partnerId) {
            var userId = GetCurrentUserId();
            await _messageService.MarkConversationAsReadAsync(userId, partnerId);
            return Ok();
        }

    }
}