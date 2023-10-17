using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CareerPlatform.DataAccess.Entities
{
    public class CV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public string City { get; set; }
        public List<JobExperience> JobExperience { get; set; } = new List<JobExperience>();
        public string? AboutMe { get; set; }
    }
}
