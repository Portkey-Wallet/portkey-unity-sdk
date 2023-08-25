using System.Collections.Generic;

namespace Portkey.Core
{
    public class AccountInfo
    {
        public string LoginAccount { get; set; } = null;
        public string Nickname { get; set; } = null;
            
        public bool IsLoggedIn()
        {
            return LoginAccount != null;
        }
    }
    
    public class AccountDetails
    {
        public string aesPrivateKey;
        public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
        public AccountInfo accountInfo = new AccountInfo();

        public void Clear()
        {
            aesPrivateKey = null;
            caInfoMap.Clear();
            accountInfo = new AccountInfo();
        }
    }
}