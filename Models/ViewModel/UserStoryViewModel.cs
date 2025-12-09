namespace Mini_Social_Media.Models.ViewModel {
    public class UserStoryViewModel {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public bool HasUnseenStories { get; set; } = true;
        public List<StoryViewModel> Stories { get; set; } = new List<StoryViewModel>();
    }

}
