using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.ViewModel {
    public class PostViewModel {
        public string Caption { get; set; } = "";
        public string Location { get; set; } = "";
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PostMediaViewModel> Medias { get; set; } = new List<PostMediaViewModel>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public List<string> Hashtags { get; set; } = new List<string>();
    }
}
