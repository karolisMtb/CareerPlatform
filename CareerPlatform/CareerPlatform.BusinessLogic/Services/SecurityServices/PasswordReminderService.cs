using CareerPlatform.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using CareerPlatform.Shared.Exceptions;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public sealed class PasswordReminderService : IPasswordReminderService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public PasswordReminderService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidatePasswordResetRequestAsync(string email, string token)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var decodedEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email));
            bool validEmail = false;

            IdentityUser user = await _userManager.FindByEmailAsync(decodedEmail);

            if(user == null)
            {
                throw new UserNotFoundException("User could not be found. Please check your email and try again");
            }

            bool validToken = await _userManager.VerifyUserTokenAsync(
                user,
                "JWT",
                "Password recovery",
                decodedToken
                );
            if(validToken == false)
            {
                validEmail = false;
                throw new ArgumentNullException("Invalid argument");
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