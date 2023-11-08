using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public sealed class LoginValidationDto
    {
        public string ValidationToken { get; private set; }
        public DateTime TokenExpirationDate { get; private set; }

        public LoginValidationDto(string validationToken, DateTime expirationDate)
        {
            ValidationToken = validationToken;
            TokenExpirationDate = expirationDate;
        }
    }
}
