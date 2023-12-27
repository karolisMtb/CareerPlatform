using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.Entities
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public Address? Address { get; set; }
        public string? AboutMe { get; set; }
        public CV? Cv { get; set; }
    }
}
