using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        //Task<User> SignUpNewUserAsync(UserSignUpDto userdto);
        //Task<User> GetUserByIdAsync(Guid userId);
        //Task<string> AuthenticateUserAsync(UserLoginDto loginDto);
        //Task<IdentityResult> DeleteUserAsync(string email);

        Task<IdentityResult> DeleteIdentityUserAsync(string email);
        Task<IdentityResult> DeleteNonIdentityUserAsync(string email);
        Task<User> GetByEmailAddressAsync(string email);
        Task<IdentityResult> CreateNonIdentityUserAsync(IdentityUser user);
        Task<IdentityResult> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword);

        Task<IdentityUser> GetUserByNameAsync(LoginModel model);
        Task<IdentityUser> GetUserByEmailAsync(string email);
    }
}