using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IEmailSender
    {
        Task ResetPasswordAsync(IdentityUser user);
        Task ConfirmUserRegistrationAsync(IdentityUser user);
    }
}
