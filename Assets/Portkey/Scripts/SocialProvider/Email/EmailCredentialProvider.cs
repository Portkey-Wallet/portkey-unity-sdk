using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailCredentialProvider : CodeCredentialProviderBase<EmailCredential, EmailAddress>
    {
        public EmailCredentialProvider(IVerifyCodeLogin emailLogin, IInternalAuthMessage message, IVerifierService verifierService, ICaptcha captcha) : base(emailLogin, message, verifierService, captcha)
        {
        }

        public override AccountType AccountType => AccountType.Email;

        protected override EmailCredential CreateCredential(string guardianId, string verifierId, string chainId, string code)
        {
            return new EmailCredential(EmailAddress.Parse(guardianId), code, chainId, verifierId);
        }
    }
}