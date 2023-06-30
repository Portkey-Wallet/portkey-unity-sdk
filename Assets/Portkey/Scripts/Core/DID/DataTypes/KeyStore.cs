namespace Portkey.Core
{
    public class KeyStore
    {
        public long Version { get; private set; }
        public string Type { get; private set; }
        public string Address { get; private set; }
        public Crypto Crypto { get; private set; }
        public string Nickname { get; private set; }
        
        public KeyStore(long version, string type, string address, Crypto crypto, string nickname)
        {
            Version = version;
            Type = type;
            Address = address;
            Crypto = crypto;
            Nickname = nickname;
        }
    }
}