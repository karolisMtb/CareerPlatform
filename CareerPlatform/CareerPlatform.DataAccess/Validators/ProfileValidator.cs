using CareerPlatform.DataAccess.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CareerPlatform.DataAccess.Validators
{
    public class ProfileValidator : AbstractValidator<UserProfile>
    {
        public ProfileValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.DateOfBirth).Must(ValidDateOfBirth).WithMessage("User can't be older than 90 years old and younger than 18.");
            RuleFor(x => x.PhoneNumber).Must(ValidPhoneNumber).WithMessage("Phone number must match Lithuania's phone number pattern.");
        }
        private bool ValidPhoneNumber(string phoneNumber)
        {
            string phoneNumberPattern = @"(?:\+370|\b8)\s?\d{3}\s?\d{5}";
            return Regex.IsMatch(phoneNumber, phoneNumberPattern);
        }

        private bool ValidDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < DateTime.UtcNow.AddYears(-90) && dateOfBirth > DateTime.UtcNow.AddYears(-18))
            {
                return false;
            }
            return true;
        }
    }
}
