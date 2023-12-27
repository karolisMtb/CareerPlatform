namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IPasswordReminderService
    {
        Task<bool> ValidatePasswordResetRequestAsync(string email, string token);
    }
}
