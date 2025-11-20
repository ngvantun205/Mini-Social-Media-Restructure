namespace Mini_Social_Media.Models.ViewModel {
    public class UserViewModel {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Bio { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string WebsiteUrl { get; set; } = "";
        public bool IsPrivate { get; set; }

        public int PostCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

        public List<PostViewModel> RecentPosts { get; set; } = new List<PostViewModel>();
    }

}
