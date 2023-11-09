﻿using CareerPlatform.DataAccess.DTOs;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IAuthenticateService
    {
        //Task<IdentityUser> NewUserAsync(RegisterModel model);
        Task<LoginValidationDto> ValidateUserLoginAsync(IdentityUser user);
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<bool> ValidateUserPasswordAsync(LoginModel model);
        Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string password);
        Task SendPasswordResetLinkAsync(IdentityUser user);
    }
}
