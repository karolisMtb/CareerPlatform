using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Repositories
{
    public class UserRepository(ApplicationDbContext _platformDbContext, IUnitOfWork _unitOfWork, UserManager<User> _userManager) : IUserRepository
    {

        //public async Task AddAsync(User user)
        //{
        //    await _platformDbContext.Users.AddAsync(user);
        //    await _unitOfWork.SaveChangesAsync();
        //}

        //public async Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto)
        //{
        //    return await _platformDbContext.Users.AnyAsync(x => x.UserName == userDto.userName || x.Email == userDto.email);
        //}

        //{
        //}

        //public async Task<User> GetByEmailAddressAsync(string email)
        //{
        //    return await _platformDbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

        //}

        //public async Task<User> GetUserByIdAsync(Guid userId)
        //{
        //    User user = await _platformDbContext.Users.Where(u => u.Id == userId.ToString()).FirstAsync(); //prideta ToString
        //    if (user == null)
        //    {
        //        throw new UserNotFoundException("User was not found.");
        //    }
        //    return user;
        //}



        //public async Task<User> GetUserByLoginCredentialsAsync(string credential)
        //{
        //    User currentUser = await _platformDbContext.Users.Where(x => x.UserName.Equals(credential) || x.Email.Equals(credential)).FirstAsync();
        //    if (currentUser == null)
        //    {
        //        throw new UserNotFoundException($"User with login '{credential}' credential was not found.");
        //    }
        //    return currentUser;
        //}
        //
    }
}
