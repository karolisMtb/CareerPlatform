using CareerPlatform.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerPlatform.DataAccess.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<BusinessUser> BusinessUsers { get; set; }
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Financial> Financials { get; set; }
        public DbSet<JobExperience> JobExperiences { get; set; }
        public DbSet<LogoImage> LogoImages { get; set; }
        public DbSet<ReviewResponse> ReviewResponses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
