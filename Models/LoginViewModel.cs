using System.ComponentModel.DataAnnotations;

namespace TasksMVC.Models
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Error.Required")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Error.Required")]
        public string Password { get; set; }
        [Display(Name = "Remerbeme")]
        public bool RememberMe { get; set; }
    }
}
