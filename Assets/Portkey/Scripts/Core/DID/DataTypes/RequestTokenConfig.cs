using System;
using System.Collections.Generic;

namespace Portkey.Core
{
    [Serializable]
    public class RequestTokenConfig
    {
        public string grant_type;
        public string client_id;
        public string scope;
        public string signature;
        public string pubkey;
        public long timestamp;
        public string ca_hash;
        public string chain_id;
    }
}