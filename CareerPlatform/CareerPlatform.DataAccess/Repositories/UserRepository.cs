using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _platformDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext platformDbContext, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _platformDbContext = platformDbContext;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        //public async Task AddAsync(User user)
        //{
        //    await _platformDbContext.Users.AddAsync(user);
        //    await _unitOfWork.SaveChangesAsync();
        //}

        //public async Task<bool> CheckIfUserExistsAsync(UserSignUpDto userDto)
        //{
        //    return await _platformDbContext.Users.AnyAsync(x => x.UserName == userDto.userName || x.Email == userDto.email);
        //}

        //public async Task<IdentityResult> DeleteUserAsync(Guid userId)
        //{
        //    var userToDelete = await _platformDbContext.Users
        //        .Where(x => x.Id == userId.ToString()) //prideta ToString()
        //        .Include(p => p.Profile).Include(p => p.Profile.Cv).Include(p => p.Profile.Address).FirstAsync();

        //    if(userToDelete is not null)
        //    {
        //        if (userToDelete.Profile is not null)
        //        {
        //            _platformDbContext.RemoveRange(userToDelete.Profile.Cv);
        //            _platformDbContext.RemoveRange(userToDelete.Profile.Address);
        //            _platformDbContext.RemoveRange(userToDelete.Profile);
        //        }

        //        _platformDbContext.RemoveRange(userToDelete);
        //    }

        //    await _unitOfWork.SaveChangesAsync();

        //    bool userExists = await _platformDbContext.Users.AnyAsync(x => x.Id == userId.ToString()); //prideta ToString()
        //    if(userExists == true)
        //    {
        //        return IdentityResult.Failed();
        //    }

        //    return IdentityResult.Success;
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
