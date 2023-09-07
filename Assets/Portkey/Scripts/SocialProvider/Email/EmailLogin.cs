using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailLogin : VerifyCodeLoginBase
    {
        public EmailLogin(IPortkeySocialService portkeySocialService) : base(portkeySocialService)
        {
        }

        public override AccountType AccountType => AccountType.Email;
        protected override bool IsCorrectGuardianIdFormat(string id, out string errormessage)
        {
            var result = LoginHelper.IsValidEmail(id);
            errormessage = result ? null : "Invalid email!";
            return result;
        }
    }
}