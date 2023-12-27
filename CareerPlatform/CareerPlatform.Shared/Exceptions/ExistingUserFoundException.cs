namespace CareerPlatform.Shared.Exceptions
{
    public class ExistingUserFoundException : Exception
    {
        public ExistingUserFoundException()
        {

        }

        public ExistingUserFoundException(string message) : base(message)
        {

        }
    }
}
