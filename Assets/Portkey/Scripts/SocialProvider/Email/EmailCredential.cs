using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class EmailCredential : ICredential
    {
        public EmailCredential(EmailAddress emailAddress, string verificationCode)
        {
            SocialInfo = new SocialInfo
            {
                sub = emailAddress.GetString
            };
            SignInToken = verificationCode;
        }
        
        public AccountType AccountType => AccountType.Email;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
    }
}