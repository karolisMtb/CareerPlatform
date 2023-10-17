using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPlatform.DataAccess.Entities
{
    public class Financial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public float AverageWage { get; set; }
        public uint Revenue { get; set; }
        public uint Debts { get; set; }

    }
}
