using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CareerPlatform.Shared.ValueObjects.enums;

namespace CareerPlatform.DataAccess.Entities
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public Ratings Rating { get; set; }
        public Company Company { get; set; }
        public User User { get; set; }
    }
}
