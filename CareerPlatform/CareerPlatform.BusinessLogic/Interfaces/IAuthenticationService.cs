using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateUserAsync(UserLoginDto loginDto);
        Task<User> SignUpNewUserAsync(UserSignUpDto userdto);
    }
}
