using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.InputModel {
    public class PostMediaInputModel {
        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        [Required]
        public string MediaType { get; set; } = "";
    }

}
