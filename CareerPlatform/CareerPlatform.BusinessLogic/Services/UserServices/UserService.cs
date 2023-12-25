using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CareerPlatform.BusinessLogic.Services.UserServices
{
    public class UserService(UserManager<User> _userManager, ILogger<User> _logger) : IUserService
    {
        public async Task<IdentityResult> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword)
        {
            User user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                _logger.LogError("User could not be found with given email.");
                throw new UserNotFoundException("User could not be found.");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if(!result.Succeeded)
            {
                _logger.LogError("Attempt to change user's password was unsuccessfull.");
                throw new PasswordChangeFailedException("Attempt to change user's password was unsuccessfull.");
            }

            return result;
        }

        public async Task<User> GetUserByEmailAddressAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Given email is null or empty.");
                throw new ArgumentNullException("Given email is null or empty.");
            }

            User user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                _logger.LogError("User could not be found with given email or email is empty.");
                throw new UserNotFoundException("User could not be found with given email or email is empty.");
            }
 
            return user;
        }

        private async Task<IdentityResult> DeleteIdentityUserAsync(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogError("User could not be found with given email or email is empty.");
                throw new UserNotFoundException("User could not be found with given email or email is empty.");
            }

            IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user);

            if (claims.Any())
            {
                await _userManager.RemoveClaimsAsync(user, claims);
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            IdentityResult deleteResult = await _userManager.DeleteAsync(user);

            if(deleteResult.Succeeded)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteUserByEmailAsync(string email, string password)
        {
            User user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                _logger.LogError("Either email or password is incorrect or null. User was not found with given data.");
                throw new UserNotFoundException("Check your credencials. There might have an error occured.");
            }

            bool validatedUser = await _userManager.CheckPasswordAsync(user, password);

            if (validatedUser is true)
            {
                IdentityResult result = await DeleteIdentityUserAsync(email);
           
                if(result is null || !result.Succeeded)
                {
                    _logger.LogError("Attempt to delete user was unsuccessfull even though user was validated.");
                    throw new InvalidOperationException("Attempt to delete user was unsuccessfull even though user was validated.");
                }
                
                return result;
            }

            return IdentityResult.Failed(
                new IdentityError()
                {
                    Description = "Failed to delete user."
                },
                new IdentityError()
                {
                    Description = "Server side error occured."
                }
            );
        }

        public async Task<User> GetUserByNameAsync(LoginModel model)
        {
            User user = await _userManager.FindByNameAsync(model.Username);

            if(user is null)
            {
                _logger.LogError("User could not be found with given creadencials.");
                throw new UserNotFoundException("User could not be found with given creadencials.");
            }

            return user;
        }

        public async Task<IdentityResult> ConfirmUserRegistrationAsync(User user, string token)
        {
            if(user is null || token.Equals(String.Empty) || token is null)
            {
                _logger.LogError("Registration could not be confirmed due to empty or null token or given user is null.");
                throw new ArgumentNullException("Registration could not be confirmed due to empty or null token or given user is null.");
            }

            return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}
