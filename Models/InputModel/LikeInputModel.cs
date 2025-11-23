using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.InputModel {
    public class LikeInputModel {
        [Required]
        public int PostId { get; set; }

    }
}
