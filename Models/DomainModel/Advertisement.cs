using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models {
    public enum AdType {
        Banner = 0,
        SponsoredPost = 1
    }

    public enum AdStatus {
        Pending = 0,  
        Approved = 1,
        Running = 2,  
        Rejected = 3, 
        Ended = 4,
        Canceled = 5
    }

    public class Advertisement {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public AdType Type { get; set; }
        public AdStatus Status { get; set; } = AdStatus.Pending;
        [Required]
        public string Title { get; set; }
        public string? Content { get; set; }
        public string ImageUrl { get; set; }
        public string? TargetUrl { get; set; }
        public string? CtaText { get; set; } = "Learn More"; 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public bool IsPaid { get; set; } = false;
        public string? AdminNote { get; set; }
        public int TotalImpressions { get; set; } = 0;
        public int TotalClicks { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; }
        public ICollection<AdStat> AdStats { get; set; } = new List<AdStat>();
    }
}