using Microsoft.AspNetCore.Mvc;

namespace CareerPlatform.DataAccess.DTOs
{
    [BindProperties]
    public class RegistrationConfirmationRequestDto
    {
        public string email { get; set; }
        public string token { get; set; }
    }
}
