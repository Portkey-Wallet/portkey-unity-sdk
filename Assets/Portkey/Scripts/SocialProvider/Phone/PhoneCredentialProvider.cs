using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider : CodeCredentialProviderBase<PhoneCredential, PhoneNumber>
    {
        public PhoneCredentialProvider(IVerifyCodeLogin phoneLogin, IInternalAuthMessage message, IVerifierService verifierService, ICaptcha captcha) : base(phoneLogin, message, verifierService, captcha)
        {
        }

        public override AccountType AccountType => AccountType.Phone;

        protected override PhoneCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new PhoneCredential(PhoneNumber.Parse(guardianId), code, chainId, verifierId);
        }
    }
}