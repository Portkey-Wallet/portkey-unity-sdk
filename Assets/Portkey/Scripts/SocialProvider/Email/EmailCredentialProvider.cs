using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailCredentialProvider
    {
        public EmailCredential Get(EmailAddress emailAddress, string verificationCode)
        {
            return new EmailCredential(emailAddress, verificationCode);
        }
    }
}