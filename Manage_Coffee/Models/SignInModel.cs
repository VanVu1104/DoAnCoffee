using System.ComponentModel.DataAnnotations;

namespace Manage_Coffee.Models
{
    public class SignInModel
    {
        [Key]
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
