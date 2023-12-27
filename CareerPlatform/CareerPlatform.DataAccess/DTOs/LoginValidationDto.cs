using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.DTOs
{
    public record LoginValidationDto(string ValidationToken, DateTime TokenExpirationDate);
    //{
    //    turi buti record
    //    public string ValidationToken { get; private set; }
    //    public DateTime TokenExpirationDate { get; private set; }

    //    public LoginValidationDto(string validationToken, DateTime expirationDate)
    //    {
    //        ValidationToken = validationToken;
    //        TokenExpirationDate = expirationDate;
    //    }
    //}
}
