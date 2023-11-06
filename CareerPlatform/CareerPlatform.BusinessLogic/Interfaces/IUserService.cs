using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> SignUpNewUserAsync(UserSignUpDto userdto);
        Task<User> GetUserByIdAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
        Task<string> AuthenticateUserAsync(UserLoginDto loginDto);
        Task<User> GetByEmailAddressAsync(string email);
    }
}