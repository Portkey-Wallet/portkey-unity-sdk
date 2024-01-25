using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class TelegramCredential : ICredential
    {
        public TelegramCredential(string accessToken, SocialInfo socialInfo)
        {
            SocialInfo = socialInfo;
            SignInToken = accessToken;
        }
        
        public AccountType AccountType => AccountType.Telegram;
        public SocialInfo SocialInfo { get; }
        public string SignInToken { get; }
    }
}