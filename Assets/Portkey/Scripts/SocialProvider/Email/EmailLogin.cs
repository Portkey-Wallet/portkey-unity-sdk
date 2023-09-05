using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailLogin : VerifyCodeLoginBase
    {
        public EmailLogin(IPortkeySocialService portkeySocialService) : base(portkeySocialService)
        {
        }

        protected override AccountType AccountType => AccountType.Email;
        protected override bool IsCorrectGuardianIdFormat(string id, out string errormessage)
        {
            var result = IsValidEmail(id);
            errormessage = result ? null : "Invalid email!";
            return result;
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