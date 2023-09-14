using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class PhoneCredential : ICodeCredential
    {
        public PhoneCredential(PhoneNumber phoneNumber, string verificationCode, string chainId, string verifierId)
        {
            SocialInfo = new SocialInfo
            {
                sub = phoneNumber.String
            };
            SignInToken = verificationCode;
            VerifierId = verifierId;
            ChainId = chainId;
        }
        
        public AccountType AccountType => AccountType.Phone;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
        public string VerifierId { get; }
        public string ChainId { get; }
    }
}