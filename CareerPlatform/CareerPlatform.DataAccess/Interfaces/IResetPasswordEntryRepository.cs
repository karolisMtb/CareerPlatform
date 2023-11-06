using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.DataAccess.Interfaces
{
    public interface IResetPasswordEntryRepository
    {
        Task AddAsync(ResetPasswordEntry resetPasswordEntry);
        Task<ResetPasswordEntry> GetResetPasswordEntryByUserId(Guid userId);
        Task InvalidateAsync(ResetPasswordEntry resetPasswordEntry);
    }
}
