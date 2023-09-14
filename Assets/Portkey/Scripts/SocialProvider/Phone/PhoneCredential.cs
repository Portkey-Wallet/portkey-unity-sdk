using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredential : ICredential
    {
        public PhoneCredential(PhoneNumber phoneNumber, string verificationCode)
        {
            SocialInfo = new SocialInfo
            {
                sub = phoneNumber.GetString
            };
            SignInToken = verificationCode;
        }
        
        public AccountType AccountType => AccountType.Phone;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
    }
}