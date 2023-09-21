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
        public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
        public SocialInfo socialInfo = new SocialInfo();

        public void Clear()
        {
            caInfoMap.Clear();
            socialInfo = new SocialInfo();
        }
    }
}