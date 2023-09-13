using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class GoogleCredential : ICredential
    {
        public GoogleCredential(string accessToken, SocialInfo socialInfo)
        {
            SocialInfo = socialInfo;
            SignInToken = accessToken;
        }
        
        public AccountType AccountType => AccountType.Google;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
    }
}