namespace Mini_Social_Media.Models.ViewModel {
    public class FeedItemViewModel {
        public string Type { get; set; }
        public int ItemId { get; set; } 
        public DateTime DisplayTime { get; set; }
        public UserSummaryViewModel Author { get; set; }
        public string? ShareCaption { get; set; }
        public PostViewModel OriginalPost { get; set; }
        public int Score { get; set; }
    }
}
