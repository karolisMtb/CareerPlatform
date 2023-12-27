namespace CareerPlatform.Shared.Exceptions
{
    public class PasswordChangeFailedException : Exception
    {
        public PasswordChangeFailedException(string message) : base(message)
        {
            
        }

        public PasswordChangeFailedException()
        {
            
        }
    }
}
