using System.Collections.Generic;

namespace Portkey.Core
{
    public class SavedAccount
    {
        public string chainId = null;
        public string aesPrivateKey = null;
        public Dictionary<string, CAInfo> caInfoMap = new Dictionary<string, CAInfo>();
        public AdditionalInfo socialInfo = new AdditionalInfo();
    }
}