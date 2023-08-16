using AElf.Types;

namespace Portkey.Core
{
    public abstract class WalletBase : ISigner
    {
        protected KeyPair KeyPair { get; set; }
        protected string PrivateKey => KeyPair.PrivateKey;
        public string Address => KeyPair.Address;
        public string PublicKey => KeyPair.PublicKey;
        
        protected WalletBase(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }
        
        public abstract Transaction SignTransaction(Transaction transaction);
        public abstract byte[] Sign(string data);
    }
}