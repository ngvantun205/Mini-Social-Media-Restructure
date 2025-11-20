using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class PostHashtag {
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; } = new Post();
        public int HashtagId { get; set; }
        [ForeignKey("HashtagId")]
        public Hashtag Hashtag { get; set; } = new Hashtag();
    }
}
