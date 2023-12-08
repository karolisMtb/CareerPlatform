using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace CareerPlatform.BusinessLogic.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        //prideti fluentvalidator
        //prideti loginima

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        //public async Task<IdentityResult> CreateNonIdentityUserAsync(User user)
        //{
        //    if(user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }

        //    User newNonIdentityuser = new User();

        //    if (newNonIdentityuser is not null)
        //    {
        //        await _userRepository.AddAsync(newNonIdentityuser);
        //        return IdentityResult.Success;
        //    }

        //    return null;
        //}

        public async Task<IdentityResult> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword)
        {
            User user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                throw new UserNotFoundException("User could not be found.");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if(!result.Succeeded)
            {
                throw new TargetInvocationException("Failed to change password. Please try again", new Exception());
            }

            return result;
        }

        public async Task<User> GetUserByEmailAddressAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Wrong data values being passed. Please check and try again");
            }

            User user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                throw new UserNotFoundException("User could not be found.");
            }
 
            return user;
        }

        private async Task<IdentityResult> DeleteIdentityUserAsync(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new UserNotFoundException("User could not be found.");
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
                throw new UserNotFoundException("Check your credencials. There might have an error occured.");
            }

            bool validatedUser = await _userManager.CheckPasswordAsync(user, password);

            if (validatedUser is true)
            {
                IdentityResult result = await DeleteIdentityUserAsync(email);
           
                if(result is null || !result.Succeeded)
                {
                    throw new InvalidOperationException("Unable to delete user. Something went wrong.");
                }
                
                return result;
            }

            return IdentityResult.Failed(
                new IdentityError()
                {
                    Description = "Failed to delete user. Check you password, please"
                },
                new IdentityError()
                {
                    Description = "Server side error"
                }
            );
        }

        public async Task<User> GetUserByNameAsync(LoginModel model)
        {
            User user = await _userManager.FindByNameAsync(model.Username);

            if(user is null)
            {
                throw new UserNotFoundException("User could not be found");
            }

            return user;
        }

        public async Task<IdentityResult> ConfirmUserRegistrationAsync(User user, string token)
        {
            if(user is null || token.Equals(String.Empty) || token is null)
            {
                throw new ArgumentNullException("Registration could no be confirmed");
            }

            return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}
