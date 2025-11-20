using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.DomainModel {
    public class PostMedia {
        [Key]
        public int PostMediaId { get; set; }
        [Required]
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; } = new Post();
        public string Url { get; set; } = "";
        public string MediaType { get; set; } = "";
        public string SortOrder { get; set; } = "";
    }
}
