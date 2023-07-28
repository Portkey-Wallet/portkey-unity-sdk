namespace Portkey.Core
{
    /// <summary>
    /// A class that holds the address, private key of a standard EOA wallet.
    /// </summary>
    public class ExternallyOwnedAccount
    {
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }
        public string PublicKey { get; private set; }
        
        public ExternallyOwnedAccount(string address, string privateKey, string publicKey)
        {
            Address = address;
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}