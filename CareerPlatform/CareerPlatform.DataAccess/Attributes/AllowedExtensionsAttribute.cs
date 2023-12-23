using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.Attributes
{
    public class AllowedExtensionsAttribute(string[] _allowedExtensions) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult("This file extension is not supported");
                }
            }
            return ValidationResult.Success;
        }
    }
}
