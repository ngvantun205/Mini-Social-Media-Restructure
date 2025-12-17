using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class ShareController : Controller {
        private readonly IShareService _shareService;
        public ShareController(IShareService shareService) {
            _shareService = shareService;
        }
        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }
        [HttpPost]
        public async Task<IActionResult> Share(ShareInputModel inputModel) {
            if(!ModelState.IsValid) return BadRequest();
            var userId = GetCurrentUserId();
            await _shareService.AddShare(inputModel, userId);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Unshare(int shareId) {
            await _shareService .DeleteShare(shareId);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> EditShare(EditShareInputModel inputModel) {
            if (!ModelState.IsValid)
                return BadRequest();
            var userId = GetCurrentUserId();
            await _shareService.EditShare(inputModel, userId);
            return Ok();
        }
    }
}
