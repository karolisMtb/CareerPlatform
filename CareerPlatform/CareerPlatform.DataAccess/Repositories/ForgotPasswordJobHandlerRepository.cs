using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;

namespace CareerPlatform.DataAccess.Repositories
{
    public class ForgotPasswordJobHandlerRepository : IForgotPasswordJobHandlerRepository
    {
        private readonly ApplicationDbContext _platformDbContext;
        private readonly IUnitOfWork _unitOfWork;
        public ForgotPasswordJobHandlerRepository(IUnitOfWork unitOfWork, ApplicationDbContext platformDbContext)
        {
            _unitOfWork = unitOfWork;
            _platformDbContext = platformDbContext;
        }

        public Task AuthenticatePasswordResetTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetPasswordResetTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(string token)
        {
            throw new NotImplementedException();
        }

        // cia reikes istraukti is ResetPasswordEntries hased tokena.
        // validuoti atsiusta i endpointa

    }
}
