using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mini_Social_Media.Controllers {
    [Authorize]
    public class ReportController : Controller {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService) {
            _reportService = reportService;
        }
        private int GetCurrentUserId() {
            string userIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdstr) ? 0 : int.Parse(userIdstr);
        }
        [HttpPost]
        public async Task<IActionResult> AddReport([FromBody] ReportInputModel inputModel) {
            try {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized();

                // Kiểm tra input
                if (inputModel == null)
                    return BadRequest("Data is null");

                var report = await _reportService.AddReport(inputModel, userId);
                return Ok(report);
            }
            catch (Exception ex) {
                // Xem lỗi này trong Output window của Visual Studio
                Console.WriteLine("Report Error: " + ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
