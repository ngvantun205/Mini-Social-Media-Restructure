namespace Mini_Social_Media.Models.ViewModel {
    public class StoryArchiveViewModel {
        public int StoryArchiveId { get; set; }
        public UserSummaryViewModel Owner { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; } = 0;
    }
}
