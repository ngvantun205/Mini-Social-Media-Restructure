using System.ComponentModel.DataAnnotations;

namespace Mini_Social_Media.Models.InputModel {
    public class RegisterInputModel {
        [Required]
        public string UserName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
    }
    public class LoginInputModel {
        [Required]
        public string? UserNameOrEmail { get; set; }

        [Required, MinLength(6)]
        public string? Password { get; set; }
    }

}
