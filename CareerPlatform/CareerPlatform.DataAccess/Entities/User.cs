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













//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using CareerPlatform.Shared.ValueObjects.enums;
//using Microsoft.AspNetCore.Identity;

//namespace CareerPlatform.DataAccess.Entities
//{
//    public class User : IdentityUser
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { get; private set; }
//        [Required]
//        public string UserName { get; private set; }
//        [Required]
//        [EmailAddress]
//        public string Email { get; private set; }
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? DisabledAt { get; set; }
//        public UserProfile? Profile { get; set; }
//        public Roles Role { get; set; } = Roles.User;
//        public IdentityUser IdentityUser { get; private set; }

//        public User(IdentityUser identityUser)
//        {
//            Id = new Guid(identityUser.Id);
//            UserName = identityUser.UserName;
//            Email = identityUser.Email;
//            IdentityUser = identityUser;
//        }

//        private User()
//        {

//        }
//    }
//}
