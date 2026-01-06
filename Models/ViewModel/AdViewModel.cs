
namespace Mini_Social_Media.Models.ViewModel {
    public class AdViewModel {
        public int Id { get; set; }
        public UserSummaryViewModel Brand { get; set; }
        public AdType Type { get; set; }
        public AdStatus Status { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public string CtaText { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public bool IsPaid { get; set; }
        public string? AdminNote { get; set; }
        public int TotalImpressions { get; set; }
        public int TotalClicks { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}
