using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public class UserPasswordDto
    {
        [Required]
        public string OldPassword { get; private set; }
        [Required]
        public string NewPassword { get; private set; }

    }
}
