using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public class UserPasswordDto
    {
        // fluent validator atstoja annotations. Pakeisti
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
        public string Email { get; set; }

    }
}
