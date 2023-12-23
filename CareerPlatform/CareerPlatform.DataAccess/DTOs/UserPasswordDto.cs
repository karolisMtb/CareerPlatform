using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public class UserPasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }

    }
}
