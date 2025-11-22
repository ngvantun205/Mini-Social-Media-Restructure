using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.DomainModel {
    public class Hashtag {
        [Key]
        public int HashtagId { get; set; }
        public string? HashtagName { get; set; }
        public int UsageCount { get; set; }
        public ICollection<PostHashtag>? PostHashtags { get; set; }
    }
}
