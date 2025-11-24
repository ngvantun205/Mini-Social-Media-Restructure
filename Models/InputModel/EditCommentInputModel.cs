using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.InputModel {
    public class EditCommentInputModel {
        [Required]
        public int CommentId { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int PostId { get; set; }
    }
}
