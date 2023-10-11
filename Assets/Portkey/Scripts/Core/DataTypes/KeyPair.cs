using AElf.HdWallet;

namespace Portkey.Core
{
    /// <summary>
    /// A class that holds the private key, corresponding public key and address.
    /// </summary>
    public class KeyPair
    {
        public string Address { get; private set; }
        public PrivateKey PrivateKey { get; private set; }
        public string PublicKey { get; private set; }
        
        public KeyPair(PrivateKey privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = privateKey.PublicKey.ToString();
            Address = privateKey.PublicKey.Decompress().ToAddress();
        }
    }
}