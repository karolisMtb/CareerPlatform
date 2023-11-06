//using CareerPlatform.BusinessLogic.Interfaces;
//using CareerPlatform.DataAccess.DTOs;
//using CareerPlatform.DataAccess.Entities;
//using Microsoft.Extensions.Configuration;
//using System.Security.Cryptography;
//using System.Text;
//using System.Web;

//namespace CareerPlatform.BusinessLogic.Services.SecurityServices
//{
//    //galimai visa sita klase ir IForgotPasswordJobHandler trinti
//    public class ForgotPasswordJobHandler : IForgotPasswordJobHandler
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IUserService _userService;
//        private readonly IPasswordReminderService _passwordReminderService;
//        private readonly IEmailService _emailService;
//        public ForgotPasswordJobHandler(
//            IConfiguration configuration, 
//            IUserService userService, 
//            IPasswordReminderService passwordReminderService,
//            IEmailService emailService)
//        {
//            _configuration = configuration;
//            _userService = userService;
//            _passwordReminderService = passwordReminderService;
//            _emailService = emailService;
//        }

//        public async Task RunAsync(string emailAddress)
//        {
//            var user = await _userService.GetByEmailAddressAsync(emailAddress);

//            if (user is null) return;

//            string token = await GenerateBase62Token(length: 64);
//            EncryptWithSha256(token, out byte[] computedHash);

//            DateTimeOffset expireDate = DateTimeOffset.UtcNow.AddMinutes(15);

//            var resetPasswordEntry = new ResetPasswordEntry()
//            {
//                HashedToken = computedHash,
//                Timestamp = expireDate,
//                User = user
//            };

//            await _passwordReminderService.AddAsync(resetPasswordEntry);

//            //string? baseUrl = _configuration["Application:BaseHost"];
//            //string resetPasswordUrl = $"{baseUrl}/api/password/reset-password?token={HttpUtility.UrlEncode(token)}";

//            ResetPasswordRequestDto resetPasswordRequest = new ResetPasswordRequestDto(
//                user.Id.ToString(), 
//                //user.Email, 
//                //resetPasswordUrl,
//                token);

//            await _emailService.SendPasswordResetEmailAsync(resetPasswordRequest);
//        }

//        private async Task<string> GenerateBase62Token(int length)
//        {
//            StringBuilder stringBuilder = new StringBuilder();
//            Random random = new Random();
//            const string base62Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

//            for (int i = 0; i < length; i++)
//            {
//                int index = random.Next(base62Chars.Length);
//                stringBuilder.Append(base62Chars[index]);
//            }
//            return stringBuilder.ToString();
//        }

//        private void EncryptWithSha256(string token, out byte[] computedHash)
//        {
//            using var hmac = new HMACSHA512();
//            computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
//        }
//    }
    
//}
