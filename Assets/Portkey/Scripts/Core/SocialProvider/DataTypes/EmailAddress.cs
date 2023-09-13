namespace Portkey.Core
{
    public class EmailAddress
    {
        public new string ToString { get; private set; }
        
        public static EmailAddress Parse(string emailAddress)
        {
            if (!IsValidEmail(emailAddress))
            {
                throw new System.Exception($"Invalid email address: {emailAddress}");
            }
            
            return new EmailAddress
            {
                ToString = emailAddress
            };
        }
        
        private static bool IsValidEmail(string email)
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