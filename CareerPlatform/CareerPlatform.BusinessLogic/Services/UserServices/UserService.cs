using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.ValueObjects.enums;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace CareerPlatform.BusinessLogic.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;

        //prideti fluentvalidator

        public UserService(
            IUserRepository userRepository,
            UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateNonIdentityUserAsync(IdentityUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            User newNonIdentityuser = new User(user);

            if(newNonIdentityuser is not null)
            {
                await _userRepository.AddAsync(newNonIdentityuser);
                return IdentityResult.Success;
            }

            return null;
        }
        public async Task<IdentityResult> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if(!result.Succeeded)
            {
                throw new TargetInvocationException("Failed to change password. Please try again", new Exception());
            }
            return result;
        }


        //public async Task<User> GetUserByIdAsync(Guid userId)
        //{
        //    if (userId == Guid.Empty)
        //    {
        //        throw new ArgumentNullException("Please specify the user you are looking for.");
        //    }

        //    return await _userRepository.GetUserByIdAsync(userId);
        //}

        public async Task<User> GetByEmailAddressAsync(string email)
        {
            if (!(email is null) || email != string.Empty)
            {
                var user = await _userRepository.GetByEmailAddressAsync(email);
                return user;
            }

            return null;
        }

        public async Task<IdentityResult> DeleteIdentityUserAsync(string email)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);

            IdentityResult roleResult = await _userManager.RemoveFromRoleAsync(user, Roles.User.ToString());
            IdentityResult deleteResult = await _userManager.DeleteAsync(user);
            
            if(roleResult.Succeeded && deleteResult.Succeeded)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteNonIdentityUserAsync(string email)
        {
            User user = await _userRepository.GetUserByLoginCredentialsAsync(email);
            IdentityResult result = await _userRepository.DeleteUserAsync(user.Id);

            return result;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser> GetUserByNameAsync(LoginModel model)
        {
            return await _userManager.FindByNameAsync(model.Username);
        }
    }
}
