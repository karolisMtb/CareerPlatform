﻿using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<string> AuthenticateUserAsync(UserLoginDto loginDto)
        {
            User currentUser = await _userRepository.GetUserByLoginCredentialsAsync(loginDto.credential);

            if (!VerifyPasswordHash(loginDto.password, currentUser.PasswordHash, currentUser.PasswordSalt))
            {
                throw new PasswordMismatchException("You entered wrong password. Please enter correct password.");
            }

            string authenticatedUserJwtToken = await GenerateTokenAsync(currentUser);

            return authenticatedUserJwtToken;
        }

        public async Task<User> SignUpNewUserAsync(UserSignUpDto userdto)
        {
            bool userExists = await CheckIfUserExistsAsync(userdto);

            if (userExists == true)
            {
                throw new ExistingUserFoundException($"User with username {userdto.userName} or email {userdto.email} already exists.");
            }

            User user = await CreateNewUserAsync(userdto);
            await _userRepository.AddAsync(user);
            return user;
        }

        private async Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto)
        {
            return await _userRepository.CheckIfUserExistsAsync(userDto);
        }

        private async Task<User> CreateNewUserAsync(UserSignUpDto userDto)
        {
            if (userDto.email == null || userDto.userName == null || userDto.password == null)
            {
                throw new ArgumentNullException("Email, password and username cannot be empty.");
            }

            CreatePasswordHash(userDto.password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User
            {
                UserName = userDto.userName,
                Email = userDto.email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }

        private async Task<string> GenerateTokenAsync(User currentUser)
        {

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256
                );


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, currentUser.UserName),
                new Claim(ClaimTypes.Email, currentUser.Email)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}