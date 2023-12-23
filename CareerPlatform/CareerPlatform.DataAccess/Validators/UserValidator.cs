using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Utilities;
using FluentValidation;

namespace CareerPlatform.DataAccess.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Email).Must(EmailValidator.ValidEmail).WithMessage("Please enter valid email address.");
        }
    }
}
