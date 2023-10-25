using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string Email { get; set; }
        [Required]
        //minimum 6digits. This includes 1 special sign and, one number and one capital letter. Validation
        public byte[] PasswordHash { get; set; } 
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DisabledAt { get; set; }
        public UserProfile? Profile { get; set; }
    }
}
