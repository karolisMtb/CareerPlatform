using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> SignUpNewUserAsync(UserSignUpDto userdto);
        Task<string> AuthenticateUserAsync(UserLoginDto loginDto);
        Task<User> GetUserByIdAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
    }
}
