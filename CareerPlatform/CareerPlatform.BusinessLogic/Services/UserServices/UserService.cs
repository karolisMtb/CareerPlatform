using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CareerPlatform.BusinessLogic.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        //prideti fluentvalidator

        public UserService(
            IUserRepository userRepository,
            ILogger<UserService> logger,
            UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<User> CreateNonIdentityUserAsync(IdentityUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            User newNonIdentityuser = new User(user);

            if(newNonIdentityuser is not null)
            {
                await _userRepository.AddAsync(newNonIdentityuser);
                return newNonIdentityuser;
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




        public async Task<User> GetByEmailAddressAsync(string email)
        {
            if (!(email is null) || email != string.Empty)
            {
                var user = await _userRepository.GetByEmailAddressAsync(email);
                return user;
            }

            return null;
        }

        public Task<IdentityResult> DeleteUserAsync(string email)
        { 
            //add all dependecies incl. Profile, foto, CV
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteIdentityUserAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
