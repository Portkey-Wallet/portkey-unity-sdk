namespace Portkey.DID
{
    public class KeyStore
    {
        public long Version { get; private set; }
        public string Type { get; private set; }
        public string Address { get; private set; }
        public Crypto Crypto { get; private set; }
        public string Nickname { get; private set; }
    }
}