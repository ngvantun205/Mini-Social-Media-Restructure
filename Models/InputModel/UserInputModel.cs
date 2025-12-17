using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.InputModel {
    public class RegisterEmailStepModel {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public class VerifyOtpStepModel {
        [Required]
        public string OtpCode { get; set; }
    }

    public class RegisterFinalStepModel {
        [Required]
        public string UserName { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string? FullName { get; set; }
        public string? Bio { get; set; }
    }
    public class SocialRegisterInputModel {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [MinLength(3, ErrorMessage = "Tên đăng nhập phải dài hơn 3 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "Chỉ chấp nhận chữ, số, dấu chấm và gạch dưới")]
        public string UserName { get; set; }

        public string? FullName { get; set; }

        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? AvatarUrl { get; set; }
    }
    public class LoginInputModel {
        [Required]
        public string? UserNameOrEmail { get; set; }

        [Required, MinLength(6)]
        public string? Password { get; set; }
    }
    public class ChangePasswordInputModel {
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        public string? ComfirmNewPassword { get; set; }   
    }
    public class PrivacyInputModel {
        public bool IsPrivate { get; set; }
    }
}
