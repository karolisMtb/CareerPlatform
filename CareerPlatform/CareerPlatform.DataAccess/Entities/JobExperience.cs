using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPlatform.DataAccess.Entities
{
    public class JobExperience
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime ExpFrom { get; set; }
        public DateTime ExpTo { get; set; }
        public string? JobDescription { get; set; }
        public Company? Company { get; set; }
    }
}
