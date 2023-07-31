namespace Portkey.Core
{
    /// <summary>
    /// A class that holds the private key, corresponding public key and address.
    /// </summary>
    public class KeyPair
    {
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }
        public string PublicKey { get; private set; }
        
        public KeyPair(string address, string privateKey, string publicKey)
        {
            Address = address;
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}