using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto);
        Task<User> GetUserByLoginCredentialsAsync(string credential);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IdentityResult> DeleteUserAsync(Guid userId);
        Task<User> GetByEmailAddressAsync(string email);
    }
}
