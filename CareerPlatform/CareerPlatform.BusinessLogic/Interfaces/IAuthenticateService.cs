using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IAuthenticateService
    {
        Task<LoginValidationDto> ValidateUserLoginAsync(User user);
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<bool> ValidateUserPasswordAsync(LoginModel model);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
        Task SendPasswordResetLinkAsync(User user);
        Task<string> DecryptConfirmationEmailAsync(string email);
        Task<string> DecryptConfirmationTokenAsync(string token);
    }
}
