using Microsoft.AspNetCore.Mvc;

namespace Mini_Social_Media.Controllers {
    public class ErrorController : Controller {
        [Route("Error/404")]
        public IActionResult Error404() {
            return View("~/Views/Shared/404.cshtml");
        }
    }

}
