using System;

namespace Portkey.Core
{
    [Serializable]
    public class ConnectToken
    {
        public string access_token;
        public string token_type;
        public string expires_in;
    }
}