using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class AppleCredential : ICredential
    {
        public AppleCredential(string identityToken, SocialInfo socialInfo)
        {
            SocialInfo = socialInfo;
            SignInToken = identityToken;
        }
        
        public AccountType AccountType => AccountType.Apple;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
    }
}