using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.ViewModel {
    public class StoryViewModel {
        public int StoryId { get; set; }
        public UserSummaryViewModel Owner { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; } = 0;
    }
}
