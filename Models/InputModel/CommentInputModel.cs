using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.InputModel {
    public class CommentInputModel {
        [Required]
        public string Content { get; set; } = "";
        [Required]
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }

}
