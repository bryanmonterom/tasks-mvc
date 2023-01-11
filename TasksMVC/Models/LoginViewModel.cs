using System.ComponentModel.DataAnnotations;

namespace TasksMVC.Models
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field must be a valid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Display(Name = "Remember Me!")]
        public bool RememberMe { get; set; }
    }
}
