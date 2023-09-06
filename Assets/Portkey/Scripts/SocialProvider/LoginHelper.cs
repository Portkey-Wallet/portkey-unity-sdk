using System.Text.RegularExpressions;

namespace Portkey.SocialProvider
{
    public static class LoginHelper
    {
        public static bool IsValidPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]*)$").Success;
        }
        
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}