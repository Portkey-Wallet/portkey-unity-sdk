using AElf.Types;

namespace Portkey.Core
{
    public abstract class AccountBase : IAccountMethods
    {
        public BlockchainWallet Wallet { get; private set; }
        public string Address  { get; private set; }
        public string PrivateKey  { get; private set; }
        
        public AccountBase(BlockchainWallet wallet)
        {
            Wallet = wallet;
            Address = wallet.Address;
            PrivateKey = wallet.PrivateKey;
        }
        
        public abstract Transaction SignTransaction(Transaction transaction);

        public abstract byte[] Sign(string data);
    }
}