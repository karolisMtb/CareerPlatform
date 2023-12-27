using CareerPlatform.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using CareerPlatform.Shared.Exceptions;
using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public sealed class PasswordReminderService(UserManager<User> _userManager) : IPasswordReminderService
    {
        public async Task<bool> ValidatePasswordResetRequestAsync(string email, string token)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var decodedEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email));
            bool validEmail = false;

            User? user = await _userManager.FindByEmailAsync(decodedEmail);

            if(user is null)
            {
                throw new UserNotFoundException("Password reminder servise could not process email or token validation.");
            }

            bool validToken = await _userManager.VerifyUserTokenAsync(
                user,
                "JWT",
                "Password recovery",
                decodedToken
                );

            if(validToken == false)
            {
                throw new ArgumentNullException("Invalid varification varificattion argument received.");
            }

            if(validToken == true)
            {
                validEmail = true;
                return validEmail;
            }

            return validEmail;
        }
    }
}