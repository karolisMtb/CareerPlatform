using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public record UserLoginDto([Required] string credential, [Required] string password);
}
