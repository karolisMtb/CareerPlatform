using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto);
        Task<User> GetUserByLoginCredentialsAsync(string credential);
        Task<User> GetUserByIdAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
        Task<User> GetByEmailAddressAsync(string email);
    }
}
