using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerPlatform.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _platformDbContext;
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(ApplicationDbContext platformDbContext, IUnitOfWork unitOfWork)
        {
            _platformDbContext = platformDbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(User user)
        {
            //user.Profile = new UserProfile()
            //{
            //    Name = "Petras",
            //    LastName = "Petriukas",
            //    DateOfBirth = new DateTime(1990, 8, 17),
            //    PhoneNumber = "8659874",

            //    Cv = new CV()
            //    {
            //        Name = "Petras",
            //        LastName = "Petriukas",
            //        PhoneNumber = "8659874",
            //        City = "Vilnius"
            //    },
            //    Address = new Address()
            //    {
            //        Country = "Lithuania",
            //        City = "kaunas",
            //        Address1 = "Liepu g. 26",
            //        ZipCode = 52545
            //    }
            //};
            //await _platformDbContext.Users.AddAsync(user);
            //await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto)
        {
            return await _platformDbContext.Users.AnyAsync(x => x.UserName == userDto.userName || x.Email == userDto.email);
        }

        public async Task<IdentityResult> DeleteUserAsync(Guid userId)
        {
            var userToDelete = await _platformDbContext.Users
                .Where(x => x.Id == userId)
                .Include(p => p.Profile).Include(p => p.Profile.Cv).Include(p => p.Profile.Address).FirstAsync();

            if(userToDelete is not null)
            {
                if (userToDelete.Profile is not null)
                {
                    _platformDbContext.RemoveRange(userToDelete.Profile.Cv);
                    _platformDbContext.RemoveRange(userToDelete.Profile.Address);
                }

                _platformDbContext.RemoveRange(userToDelete.Profile);
                _platformDbContext.RemoveRange(userToDelete);
            }

            await _unitOfWork.SaveChangesAsync();

            bool userExists = await _platformDbContext.Users.AnyAsync(x => x.Id == userId);
            if(userExists == true)
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        public async Task<User> GetByEmailAddressAsync(string email)
        {
            return await _platformDbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            User user = await _platformDbContext.Users.Where(u => u.Id == userId).FirstAsync();
            if (user == null)
            {
                throw new UserNotFoundException("User with your requested id is not found.");
            }

            return user;
        }

        public async Task<User> GetUserByLoginCredentialsAsync(string credential)
        {
            User currentUser = await _platformDbContext.Users.Where(x => x.UserName == credential || x.Email == credential).FirstAsync();

            if (currentUser == null)
            {
                throw new UserNotFoundException($"User with login '{credential}' was not found.");
            }
            return currentUser;
        }
    }
}
