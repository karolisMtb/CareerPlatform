using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByNameAsync(LoginModel model);
        Task<User> GetUserByEmailAddressAsync(string email);
        Task<IdentityResult> DeleteUserByEmailAsync(string email, string password);
        Task<IdentityResult> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword);
        Task<IdentityResult> ConfirmUserRegistrationAsync(User user, string token);
    }
}