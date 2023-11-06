using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerPlatform.DataAccess.Repositories
{
    public class ResetPasswordEntryRepository : IResetPasswordEntryRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _platformDbContext;

        public ResetPasswordEntryRepository(IUnitOfWork unitOfWork, ApplicationDbContext platformDbContext)
        {
            _unitOfWork = unitOfWork;
            _platformDbContext = platformDbContext;
        }

        public async Task AddAsync(ResetPasswordEntry resetPasswordEntry)
        {
            if(resetPasswordEntry is null)
            {
                throw new ArgumentNullException("ResetPasswordEntry is null. Null can't be added to database.");
            }

            await _platformDbContext.ResetPasswordEntries.AddAsync(resetPasswordEntry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ResetPasswordEntry> GetResetPasswordEntryByUserId(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException("Wrong user identified");
            }

            ResetPasswordEntry? resetPasswordEntry = await _platformDbContext.ResetPasswordEntries
                .Where(x => x.User.Id.Equals(userId)).FirstOrDefaultAsync();

            if(resetPasswordEntry is null)
            {
                throw new ResetPasswordEntryException("Password reset entry was not found");
            }
            
            return resetPasswordEntry;
        }

        public async Task InvalidateAsync(ResetPasswordEntry resetPasswordEntry)
        {
            _platformDbContext.ResetPasswordEntries.Remove(resetPasswordEntry);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
