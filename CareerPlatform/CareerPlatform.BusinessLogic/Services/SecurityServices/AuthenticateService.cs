﻿using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        public AuthenticateService(IConfiguration configuration, 
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        //add logger

        private async Task<List<Claim>> GetUserClaimsAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = await GetAuthenticationClaimsAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return claims;
        }

        private async Task<List<Claim>> GetAuthenticationClaimsAsync(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return authClaims;
        }

        private async Task<JwtSecurityToken> GetTokenAsync(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddMinutes(15),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private async Task<User> NewUserAsync(RegisterModel model)
        {
            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            return user;
        }

        public async Task<LoginValidationDto> ValidateUserLoginAsync(User user)
        {
            List<Claim> claims = await GetUserClaimsAsync(user);
            JwtSecurityToken token = await GetTokenAsync(claims);

            var authToken = new JwtSecurityTokenHandler().WriteToken(token);
            var expiration = token.ValidTo;
            LoginValidationDto loginValidationDto = new(authToken, expiration);

            return loginValidationDto;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            User userByName = await _userManager.FindByNameAsync(model.Username);
            User userByEmail = await _userManager.FindByEmailAsync(model.Email);

            if (userByName is not null  || userByEmail is not null)
            {
                throw new ExistingUserFoundException("User already exists with this credential. Try another one, please");
            }

            User user = await NewUserAsync(model);
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if(!result.Succeeded)
            {
                return IdentityResult.Failed(

                    new IdentityError()
                    {
                        Description = "Invalid username or password"
                    },
                    new IdentityError()
                    {
                        Description = "Server side error"
                    }
                );
            }

            await _emailSender.ConfirmUserRegistrationAsync(user);
                
            return IdentityResult.Success;
         
        }

        public async Task<bool> ValidateUserPasswordAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if(user is null)
            {
                throw new UserNotFoundException("Either wrong input or user is not registered.");
            }

            bool validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            
            return validPassword;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, password);

            if(!result.Succeeded)
            {
                throw new Exception("Error occured while resetting your password.");
            }

            return result;
        }

        public async Task SendPasswordResetLinkAsync(User user)
        {
            if(user is null)
            {
                throw new ArgumentNullException("Error occured while sending reset password link.");
            }

           await _emailSender.ResetPasswordAsync(user);           
        }

        public async Task SendGridTest()
        {
            var user = await _userManager.FindByEmailAsync("karolis.mtb@gmail.com");

            await _emailSender.ConfirmUserRegistrationAsync(user);
        }

        public async Task<string> DecryptConfirmationEmailAsync(string email)
        {
            string decryptedEmail = await DecryptAsync(email);

            if(string.IsNullOrEmpty(decryptedEmail))
            {
                throw new ArgumentNullException("Email could not be revealed");
            }

            return decryptedEmail;
        }

        public async Task<string> DecryptConfirmationTokenAsync(string token)
        {
            string decryptedToken = await DecryptAsync(token);
            if (string.IsNullOrEmpty(decryptedToken))
            {
                throw new ArgumentNullException("Email could not be revealed");
            }

            return decryptedToken;
        }

        private static async Task<string> DecryptAsync(string data)
        {
            var base64Encoded = Convert.FromBase64String(data);
            string decryptedData = Encoding.UTF8.GetString(base64Encoded);
            return decryptedData;
        }
    }
}