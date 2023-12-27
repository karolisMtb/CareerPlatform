using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Entities;
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
        private readonly UserManager<User> _userManager;

        public EmailSender(IConfiguration configuration,
            UserManager<User> userManager)
        {
            _configuration = configuration;
            _client = new SendGridClient(_configuration["EmailSender:ApiKey"]);
            _userManager = userManager;
        }

        public async Task ConfirmUserRegistrationAsync(User user)
        {
            string baseUrl = _configuration["Application:BaseHost"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
            var callbackUrl = $"{baseUrl}/api/authenticate/confirm-registration?email={encodedEmail}&token={encodedToken}";
            var response = await SendEmailAsync(user.Email, "Registration confirmation", $"Please confirm your account registration by <a href='{callbackUrl}'>clicking here: <a/><p>{callbackUrl}</p>");

             Console.WriteLine(response);
        }

        public async Task ResetPasswordAsync(User user)
        {
            string baseUrl = _configuration["Application:BaseHost"];
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
            var callbackUrl = $"{baseUrl}/api/authenticate/reset-password?email={encodedEmail}&token={encodedToken}";
            var response = await SendEmailAsync(user.Email, "Reset your password",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here: {callbackUrl}</a>");

            Console.WriteLine(response);
        }

        //private async Task<Response> SendEmailAsync(string email, string subj, string htmlMessage)
        //{
        //    var _clients = new SendGridClient(_configuration["EmailSender:ApiKey"]);
        //    if (_client is null)
        //    {
        //        throw new NullReferenceException("Client is null");
        //    }
        //    var from = new EmailAddress(email, "Testing shit");
        //    var subject = subj;
        //    var to = new EmailAddress("karolis.mtb@gmail.com", "Example User");
        //    var plainTextContent = "Testing email service 1";
        //    var htmlContent = htmlMessage;
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await _clients.SendEmailAsync(msg);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Response exception");
        //    }

        //    Console.WriteLine(response.StatusCode);
        //    Console.WriteLine(response.Body.ReadAsStringAsync());
        //    return response;
        //}



        private async Task<Response> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var from = new EmailAddress(_configuration["EmailSender:SenderEmail"],
                _configuration["EmailSender:SenderName"]);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            var response = await _client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Response exception");
            }

            return response;
        }
    }
}
