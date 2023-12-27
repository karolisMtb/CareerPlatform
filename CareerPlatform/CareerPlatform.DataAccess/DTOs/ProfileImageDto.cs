using CareerPlatform.DataAccess.Attributes;
using Microsoft.AspNetCore.Http;

namespace CareerPlatform.DataAccess.DTOs
{
    public record ProfileImageDto
    (
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { "jpeg", "jpg", "png" })]
        IFormFile ProfileImage
    );
}
