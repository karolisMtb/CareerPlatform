using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace CareerPlatform.BusinessLogic.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<UserService> _logger;

        //prideti fluentvalidator

        public UserService(
            IUserRepository userRepository,
            IAuthenticationService authenticationService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("Please specify the user you are looking for.");
            }

            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("You have entered wrong credentials. Please try again.");
            }

            await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<User> SignUpNewUserAsync(UserSignUpDto userdto)
        {
            return await _authenticationService.SignUpNewUserAsync(userdto);
        }

        public async Task<string> AuthenticateUserAsync(UserLoginDto loginDto)
        {
            return await _authenticationService.AuthenticateUserAsync(loginDto);
        }

        public async Task<User> GetByEmailAddressAsync(string email)
        {
            if(!(email is null) || email != string.Empty)
            {
                var user = await _userRepository.GetByEmailAddressAsync(email);
                return user;
            }

            return null;
        }
    }
}
