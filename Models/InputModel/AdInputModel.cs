using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models.InputModel {
    public class AdInputModel {
        [Required(ErrorMessage = "Vui lòng nhập tên chiến dịch")]
        public string Title { get; set; }

        public string? Content { get; set; } 

        [Required(ErrorMessage = "Vui lòng chọn ảnh quảng cáo")]
        public IFormFile AdMedia { get; set; }

        [Required]
        [Url(ErrorMessage = "Đường dẫn không hợp lệ")]
        public string TargetUrl { get; set; }

        public string CtaText { get; set; } = "Xem thêm";

        public AdType Type { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    }
    public class EditAdInputModel {
        public int Id { get; set; }  
        public AdType Type { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public IFormFile AdMedia { get; set; }
        public string? TargetUrl { get; set; }
        public string? CtaText { get; set; } = "Learn More";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
    }
}
