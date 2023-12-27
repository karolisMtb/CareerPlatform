using CareerPlatform.Shared.ValueObjects.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPlatform.DataAccess.Entities
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public LegalTypes Legaltype { get; set; }
        public string? Director { get; set; }
        public string? CEO { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime EstablishedAt { get; set; }
        public LogoImage? Logo { get; set; }
        public Address Address { get; set; }
        public int NumberOfEmployees { get; set; }
        public List<Review>? Reviews { get; set; }
        public Financial Financials { get; set; }

    }
}
