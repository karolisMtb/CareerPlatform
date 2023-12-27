using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPlatform.DataAccess.Entities
{
    public class BusinessProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<ReviewResponse>? Responses { get; set; }

    }
}
