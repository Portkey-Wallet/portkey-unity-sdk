using System.Collections.Generic;

namespace Portkey.Core
{
    public class AdditionalInfo
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
        public string chainId = null;
        public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
        public AdditionalInfo socialInfo = new AdditionalInfo();

        public void Clear()
        {
            caInfoMap.Clear();
            socialInfo = new AdditionalInfo();
        }
    }
}