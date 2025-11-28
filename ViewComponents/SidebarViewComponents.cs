using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mini_Social_Media.ViewComponents {
    public class SidebarViewComponent : ViewComponent {
        private readonly UserManager<User> _userManager;
        private readonly INotificationsRepository _notiRepo;

        public SidebarViewComponent(UserManager<User> userManager, INotificationsRepository notiRepo) {
            _userManager = userManager;
            _notiRepo = notiRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var model = new SidebarViewModel();

            model.CurrentController = RouteData.Values["controller"]?.ToString() ?? "";
            model.CurrentAction = RouteData.Values["action"]?.ToString() ?? "";

            if (User.Identity != null && User.Identity.IsAuthenticated) {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null) {
                    if (!string.IsNullOrEmpty(user.AvatarUrl)) {
                        model.AvatarUrl = user.AvatarUrl;
                    }

                    var notis = await _notiRepo.GetByReceiverIdAsync(user.Id);
                    model.HasUnread = notis.Any(n => !n.IsRead);
                }
            }
            return View(model);
        }
    }
}