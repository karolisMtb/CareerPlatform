using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public sealed class PasswordReminderService : IPasswordReminderService
    {
        private readonly IResetPasswordEntryRepository _resetPasswordEntryRepository;
        public PasswordReminderService(IResetPasswordEntryRepository resetPasswordEntryRepository)
        {
            _resetPasswordEntryRepository = resetPasswordEntryRepository;
        }

        public async Task AddAsync(ResetPasswordEntry resetPasswordEntry)
        {
            await _resetPasswordEntryRepository.AddAsync(resetPasswordEntry);
        }

        public async Task<string> DecodeHashedTokenAsync(byte[] hashedToken)
        {
            if(hashedToken == null)
            {
                throw new ArgumentNullException("No token was given.");
            }

            using var hmac = new HMACSHA256();
            var decodedToken = hmac.ComputeHash(hashedToken);
            return Encoding.UTF8.GetString(decodedToken);
        }

        public async Task<ResetPasswordEntry> GetByUserIdAsync(Guid userId)
        {
            return await _resetPasswordEntryRepository.GetResetPasswordEntryByUserId(userId);
        }

        public async Task InvalidateAsync(ResetPasswordEntry resetPasswordEntry)
        {
            await _resetPasswordEntryRepository.InvalidateAsync(resetPasswordEntry);
        }
    }
}

//Token Generation:

//Method to generate random tokens with options for length and character set (e.g., alphanumeric, Base64, Base62).
//Method to generate secure random passwords.
//Password Hashing:

//Methods to hash passwords using different algorithms (e.g., SHA - 256, bcrypt, Argon2).
//Methods to verify password hashes against user input.
//Token/Password Validation:

//Method to validate tokens or passwords based on certain criteria (e.g., length requirements, character set requirements).
//Expiry and Time-Related Functions:

//Methods to set and check the expiry date of tokens or passwords.
//Functions to calculate time differences (e.g., remaining time until expiry).
//Salting(for password hashing):

//Functions to generate and manage unique salts for each password.
//Other Security Features:

//Security - related configurations, like iterations for password hashing algorithms.
//Functions for comparing and timing constant-time comparisons for security purposes.
//Error Handling:

//Handling exceptions and error reporting for various operations.
//Configuration:

//Configuration options for different aspects, such as hashing algorithms and default settings.