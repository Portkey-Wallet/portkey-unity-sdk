using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailCredential : ICodeCredential
    {
        public EmailCredential(EmailAddress emailAddress, string verificationCode, string chainId, string verifierId)
        {
            SocialInfo = new SocialInfo
            {
                sub = emailAddress.String
            };
            SignInToken = verificationCode;
            VerifierId = verifierId;
            ChainId = chainId;
        }
        
        public AccountType AccountType => AccountType.Email;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
        public string VerifierId { get; }
        public string ChainId { get; }
    }
}