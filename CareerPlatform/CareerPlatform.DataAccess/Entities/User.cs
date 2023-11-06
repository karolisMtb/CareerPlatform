using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CareerPlatform.Shared.ValueObjects.enums;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //[Required]
        //[MinLength(12)]
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DisabledAt { get; set; }
        public UserProfile? Profile { get; set; }
        public Roles Role { get; set; } = Roles.User;
    }
}
