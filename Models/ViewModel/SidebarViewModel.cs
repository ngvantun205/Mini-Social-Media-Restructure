namespace Mini_Social_Media.Models.ViewModel {
    public class SidebarViewModel {
        public string AvatarUrl { get; set; } = "/images/avatar.png";
        public bool HasUnread { get; set; }
        public string CurrentController { get; set; } = "";
        public string CurrentAction { get; set; } = "";
    }
}
