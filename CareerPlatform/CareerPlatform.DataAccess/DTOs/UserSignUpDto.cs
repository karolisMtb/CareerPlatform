using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    //for dtos we use records
    public record UserSignUpDto([Required] string userName, [Required] string password, [Required] string email);
}
