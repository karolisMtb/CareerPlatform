using CareerPlatform.DataAccess.Entities;

namespace CareerPlatform.DataAccess.Interfaces
{
    public interface IForgotPasswordJobHandlerRepository
    {
        Task<byte[]> GetPasswordResetTokenAsync(string token);
        Task AuthenticatePasswordResetTokenAsync(string token);
        Task<User> GetUserByIdAsync(string token);
    }
}
