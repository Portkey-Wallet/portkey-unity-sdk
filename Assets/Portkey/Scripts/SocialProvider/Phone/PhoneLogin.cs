using System.Text.RegularExpressions;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneLogin : VerifyCodeLoginBase
    {
        public PhoneLogin(IPortkeySocialService portkeySocialService) : base(portkeySocialService)
        {
        }

        public override AccountType AccountType => AccountType.Phone;
        protected override bool IsCorrectGuardianIdFormat(string id, out string errormessage)
        {
            var result = LoginHelper.IsValidPhoneNumber(id);
            errormessage = result ? null : "Invalid phone number!";
            return result;
        }
    }
}