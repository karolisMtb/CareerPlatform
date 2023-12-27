using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DisabledAt { get; set; }
        public UserProfile? Profile { get; set; }
    }
}