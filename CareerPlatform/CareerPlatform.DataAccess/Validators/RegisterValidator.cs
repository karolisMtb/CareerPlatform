using CareerPlatform.DataAccess.Utilities;
using FluentValidation;
using JWTAuthentication.NET6._0.Auth;
using System.Text.RegularExpressions;

namespace CareerPlatform.DataAccess.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterModel>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("5");
            RuleFor(x => x.Email).Must(EmailValidator.ValidEmail).WithMessage("Please enter valid email address.");
            RuleFor(x => x.Password).Must(BeValidPassword).WithMessage("Password must be at least 8 characters long, containing at least 1 special character and capital letter.");
        }

        private bool BeValidPassword(string password)
        {
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
