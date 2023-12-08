using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Interfaces;

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

    }
}
