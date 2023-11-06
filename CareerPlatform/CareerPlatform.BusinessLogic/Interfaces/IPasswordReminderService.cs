using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IPasswordReminderService
    {
        Task AddAsync(ResetPasswordEntry resetPasswordEntry);
        Task<ResetPasswordEntry> GetByUserIdAsync(Guid userId);
        Task<string> DecodeHashedTokenAsync(byte[] hashedToken);
        Task InvalidateAsync(ResetPasswordEntry resetPasswordEntry);
    }
}
