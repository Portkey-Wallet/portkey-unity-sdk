using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredentialProvider
    {
        public PhoneCredential Get(PhoneNumber phoneNumber, string verificationCode)
        {
            return new PhoneCredential(phoneNumber, verificationCode);
        }
    }
}