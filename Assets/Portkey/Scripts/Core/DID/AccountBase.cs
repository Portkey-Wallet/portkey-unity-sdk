using AElf.Types;

namespace Portkey.Core
{
    public abstract class AccountBase : ISigner
    {
        public ExternallyOwnedAccount Wallet { get; private set; }
        public string Address => Wallet.Address;
        public string PrivateKey => Wallet.PrivateKey;
        public string PublicKey => Wallet.PublicKey;
        
        public AccountBase(ExternallyOwnedAccount wallet)
        {
            Wallet = wallet;
        }
        
        public abstract Transaction SignTransaction(Transaction transaction);

        public abstract byte[] Sign(string data);
    }
}