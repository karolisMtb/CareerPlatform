using FluentValidation;
using JWTAuthentication.NET6._0.Auth;

namespace CareerPlatform.DataAccess.Validators
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password ir required.");
        }
    }
}
