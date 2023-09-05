using System.Text.RegularExpressions;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneLogin : VerifyCodeLoginBase
    {
        public PhoneLogin(IPortkeySocialService portkeySocialService) : base(portkeySocialService)
        {
        }

        protected override AccountType AccountType => AccountType.Phone;
        protected override bool IsCorrectGuardianIdFormat(string id, out string errormessage)
        {
            var result = IsValidPhoneNumber(id);
            errormessage = result ? null : "Invalid phone number!";
            return result;
        }
        
        private static bool IsValidPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]*)$").Success;
        }
    }
}