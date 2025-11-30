using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mini_Social_Media.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) {
            _adminService = adminService;
        }
        public IActionResult Index() {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Users() {
            var users = await _adminService.GetAllUser();
            if (users == null)
                return Ok(new { success = false, error = "No user found" });
            return Ok(users);
        }
        [HttpGet]
        public async Task<IActionResult> SearchUser(string searchinfo) {
            if (string.IsNullOrEmpty(searchinfo))
                return Ok(new { success = false, errormessage = "Search infomation cannot null" });
            var users = await _adminService.SearchUser(searchinfo);
            if (users == null)
                return Ok(new List<UserSummaryViewModel>());
            return Ok(users);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] int userId) {
            await _adminService.DeleteUser(userId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Posts() {
            var posts = await _adminService.GetAllPosts();
            if (posts == null)
                return Ok(new { success = false, errormessage = "No post found" });
            return Ok(posts);
        }
        [HttpGet]
        public async Task<IActionResult> SearchPost(string searchinfo) {
            if (string.IsNullOrEmpty(searchinfo))
                return Ok(new { success = false, errormessage = "Search infomation cannot null" });
            var posts = await _adminService.SearchPosts(searchinfo);
            if (posts == null)
                return Ok(new List<PostSummaryViewModel>());
            return Ok(posts);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost([FromBody] int postId) {
            await _adminService.DeletePost(postId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Reports() {
            var reports = await _adminService.GetAllReports();
            if (reports == null)
                return Ok(new { success = false, errormessage = "No report found" });
            return Ok(reports);
        }
        [HttpPost]
        public async Task<IActionResult> ExecuteReport([FromBody] int reportId) {
            await _adminService.ExecuteReport(reportId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetReportsByStatus(string status) {
            if (!Enum.TryParse(status, true, out ReportStatus statusEnum)) {
                return BadRequest("Status must be 'Pending' or 'Executed'");
            }
            var reports = await _adminService.FilterReportsByStatus(statusEnum);

            if (reports == null || !reports.Any())
                return Ok(new { success = false, message = "No report found" });

            return Ok(reports);
        }
    }
}
