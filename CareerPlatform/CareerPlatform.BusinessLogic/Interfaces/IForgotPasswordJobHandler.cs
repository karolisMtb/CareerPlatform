namespace CareerPlatform.BusinessLogic.Interfaces
{
    public interface IForgotPasswordJobHandler
    {
        Task RunAsync(string emailAddress);
    }
}
