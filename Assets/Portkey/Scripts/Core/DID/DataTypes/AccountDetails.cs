using System.Collections.Generic;

namespace Portkey.Core
{
    public class SocialInfo
    {
        public string LoginAccount { get; set; } = null;
        public string Nickname { get; set; } = null;
            
        public bool Exists()
        {
            return LoginAccount != null;
        }
    }
    
    public class AccountDetails
    {
        public string aesPrivateKey = null;
        public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
        public SocialInfo socialInfo = new SocialInfo();

        public void Clear()
        {
            aesPrivateKey = null;
            caInfoMap.Clear();
            socialInfo = new SocialInfo();
        }
    }
}