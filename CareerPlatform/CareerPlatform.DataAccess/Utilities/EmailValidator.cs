using System.Text.RegularExpressions;

namespace CareerPlatform.DataAccess.Utilities
{
    public static class EmailValidator
    {
        public static bool ValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
