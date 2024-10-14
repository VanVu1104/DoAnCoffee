using System.ComponentModel.DataAnnotations;

namespace Manage_Coffee.Models
{
    public class ForgotPassword
    {
        [Key]
        [Required, EmailAddress]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
