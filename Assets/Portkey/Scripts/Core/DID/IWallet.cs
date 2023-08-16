using AElf.Types;

namespace Portkey.Core
{
    public abstract class IWallet : ISigner
    {
        protected KeyPair KeyPair { get; set; }
        protected string PrivateKey => KeyPair.PrivateKey;
        public string Address => KeyPair.Address;
        public string PublicKey => KeyPair.PublicKey;
        
        public abstract Transaction SignTransaction(Transaction transaction);

        public abstract byte[] Sign(string data);
    }
}