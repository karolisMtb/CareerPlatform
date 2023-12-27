namespace CareerPlatform.Shared.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message)
        {

        }
        public UserNotFoundException()
        {

        }
    }
}
