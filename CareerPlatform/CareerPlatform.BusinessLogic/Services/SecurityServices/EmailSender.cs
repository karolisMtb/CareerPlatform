using CareerPlatform.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly SendGridClient _client;
        private readonly UserManager<IdentityUser> _userManager;

        public EmailSender(IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _client = new SendGridClient(_configuration["EmailSender:ApiKey"]);
            _userManager = userManager;
        }

        public async Task ResetPasswordAsync(IdentityUser user)
        {
            string baseUrl = _configuration["Application:BaseHost"];
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
            var callbackUrl = $"{baseUrl}/api/authenticate/reset-password?email={encodedEmail}&token={encodedToken}";
            await SendEmailAsync(user.Email, "Reset your password",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>");
        }

        private async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var from = new EmailAddress(_configuration["EmailSender:SenderEmail"],
                _configuration["EmailSender:SenderName"]);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await _client.SendEmailAsync(msg);
        }
    }
}
