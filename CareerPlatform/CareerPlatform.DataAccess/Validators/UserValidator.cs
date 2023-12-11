using CareerPlatform.DataAccess.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CareerPlatform.DataAccess.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Email).Must(ValidEmail).WithMessage("Please enter valid email address.");
            RuleFor(x => x.Profile.Name).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.Profile.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Profile.DateOfBirth).Must(ValidDateOfBirth).WithMessage("User can't be older than 90 years old and younger than 18.");
            RuleFor(x => x.Profile.PhoneNumber).Must(ValidPhoneNumber).WithMessage("Phone number must match Lithuania's phone number pattern.");
            RuleFor(x => x.Profile.Address.Address1).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.Profile.Address.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Profile.Address.Country).NotEmpty().WithMessage("Country is required.");
        }

        private bool ValidPhoneNumber(string phoneNumber)
        {
            string phoneNumberPattern = @"(?:\+370|\b8)\s?\d{3}\s?\d{5}";
            return Regex.IsMatch(phoneNumber, phoneNumberPattern);
        }

        private bool ValidDateOfBirth(DateTime dateOfBirth)
        {
            if(dateOfBirth < DateTime.UtcNow.AddYears(-90) && dateOfBirth > DateTime.UtcNow.AddYears(-18)){
                return false;
            }
            return true;
        }

        private bool ValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
