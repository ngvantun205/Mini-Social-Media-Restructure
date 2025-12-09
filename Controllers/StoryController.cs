using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class StoryController : Controller {
        private readonly IStoryService _storyService;

        public StoryController(IStoryService storyService) {
            _storyService = storyService;
        }

        private int GetCurrentUserId() {
            var userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdstr == null ? 0 : int.Parse(userIdstr);
        }
        [HttpGet]
        public IActionResult Create() {
            return View("Create");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] StoryInputModel storyInputModel) {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View("Create");
            var result = await _storyService.AddStory(storyInputModel, userId);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStory(int storyId) {
            await _storyService.DeleteStory(storyId);
            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Stories() {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var result = await _storyService.GetCurrentStories(userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> StoryArchives() {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var archives = await _storyService.GetUserStoryArchives(userId);
            return Ok(archives ?? new List<StoryArchiveViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArchive(int archiveId)
        {
            await _storyService.DeleteArchive(archiveId);
            return Ok(new { success = true });
        }
    }
}