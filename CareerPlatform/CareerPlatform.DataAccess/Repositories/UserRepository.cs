using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace CareerPlatform.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PlatformDbContext _platformDbContext;
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(PlatformDbContext platformDbContext, IUnitOfWork unitOfWork)
        {
            _platformDbContext = platformDbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(User user)
        {
            await _platformDbContext.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsyn();
        }

        public async Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto)
        {
            return await _platformDbContext.Users.AnyAsync(x => x.UserName == userDto.userName || x.Email == userDto.email);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var userToDelete = await _platformDbContext.Users
                .Where(x => x.Id == userId)
                .Include(p => p.Profile).FirstOrDefaultAsync();

            if(userToDelete != null)
            {
                if(userToDelete.Profile != null)
                {
                    _platformDbContext.Remove(userToDelete.Profile.Cv);
                    _platformDbContext.Remove(userToDelete.Profile.Address);
                }

                _platformDbContext.Remove(userToDelete.Profile);
                _platformDbContext.Remove(userToDelete);
            }

            await _unitOfWork.SaveChangesAsyn();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            User user = await _platformDbContext.Users.Where(u => u.Id == userId).FirstAsync();
            if (user == null)
            {
                throw new UserNotFountException("User with your requested id is not found.");
            }

            return user;
        }

        public async Task<User> GetUserByLoginCredentialsAsync(string credential)
        {
            User currentUser = await _platformDbContext.Users.Where(x => x.UserName == credential || x.Email == credential).FirstAsync();

            if (currentUser == null)
            {
                throw new UserNotFountException($"User with login '{credential}' was not found.");
            }
            return currentUser;
        }
    }
}
