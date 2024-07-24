using System.ComponentModel.DataAnnotations;

namespace FileSharingWebsite.Entities.Models
{
    public class SignUpViewModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? RePassword { get; set; }
    }
}
