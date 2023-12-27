using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CareerPlatform.DataAccess.Repositories
{
    public class UserRepository(ApplicationDbContext _platformDbContext, IUnitOfWork _unitOfWork, UserManager<User> _userManager) : IUserRepository
    {

    }
}
