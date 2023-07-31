using AElf.Types;

namespace Portkey.Core
{
    public abstract class AccountBase : ISigner
    {
        public KeyPair KeyPair { get; private set; }
        public string Address => KeyPair.Address;
        public string PrivateKey => KeyPair.PrivateKey;
        public string PublicKey => KeyPair.PublicKey;
        
        public AccountBase(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }
        
        public abstract Transaction SignTransaction(Transaction transaction);

        public abstract byte[] Sign(string data);
    }
}