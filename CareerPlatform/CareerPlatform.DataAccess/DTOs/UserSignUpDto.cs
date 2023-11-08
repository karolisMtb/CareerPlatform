using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public record UserSignUpDto([Required] string userName, [Required] string password, [Required] string email);
}
