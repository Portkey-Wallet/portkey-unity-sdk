using System.Text.RegularExpressions;
using Portkey.Core;

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
        
        public static VerificationDoc ProcessVerificationDoc(string verificationDoc, string verifierId)
        {
            var documentValue = verificationDoc.Split(',');
            var verificationDocObj = new VerificationDoc
            {
                verifierId = verifierId,
                type = documentValue[0],
                identifierHash = documentValue[1],
                verificationTime = documentValue[2],
                verifierAddress = documentValue[3],
                salt = documentValue[4],
                toString = verificationDoc
            };
            return verificationDocObj;
        }
    }
}