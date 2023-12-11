using System;

namespace Portkey.Core
{
    [Serializable]
    public class ConnectToken
    {
        public string access_token;
        public long expires_in;
    }
}