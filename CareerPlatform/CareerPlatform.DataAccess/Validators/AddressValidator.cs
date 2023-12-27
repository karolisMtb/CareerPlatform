using CareerPlatform.DataAccess.Entities;
using FluentValidation;

namespace CareerPlatform.DataAccess.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Address1).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
        }
    }
}
