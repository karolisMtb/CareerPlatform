using CareerPlatform.DataAccess.Entities;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IEmailSender
    {
        Task ResetPasswordAsync(User user);
        Task ConfirmUserRegistrationAsync(User user);
    }
}
