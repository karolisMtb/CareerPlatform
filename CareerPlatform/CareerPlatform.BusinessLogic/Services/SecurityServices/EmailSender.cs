using CareerPlatform.BusinessLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly SendGridClient _client;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new SendGridClient(_configuration["EmailSender:ApiKey"]);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var from = new EmailAddress(_configuration["EmailSender:SenderEmail"],
                _configuration["EmailSender:SenderName"]);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await _client.SendEmailAsync(msg);
        }
    }
}
